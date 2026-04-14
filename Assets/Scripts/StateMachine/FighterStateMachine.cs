using UnityEngine;

public class FighterStateMachine : MonoBehaviour
{
    public FighterBaseState CurrentState { get; set; }

    public BasicMovementDatas basicMovementDatas;

    public bool isDummy = false;

    public Animator animator;


    enum FacingDirection
    {
        Left = -1,
        Right = 1,
        Neutral = 0
    }


    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public bool fastFallPressed;
    [HideInInspector] public bool jumpPressed;
    [HideInInspector] public bool isGrounded;

    [HideInInspector] public bool lightPressed;
    [HideInInspector] public bool heavyPressed;

    public AttackData[] lightComboAttacks;
    public AttackData[] heavyComboAttacks;

    [Header("Dash Cooldown")]
    public float dashCooldown = 0.4f;
    [HideInInspector] public float dashCooldownTimer = 0f;



    [HideInInspector] public int dashDirection; // +1 forward, -1 back
    [HideInInspector] public float lastForwardTap;
    [HideInInspector] public float lastBackTap;

    public float doubleTapThreshold = 0.25f;
    [HideInInspector] public bool airDashUsed;

    public float dashSpeed = 25f;
    public float dashDuration = 0.1f;

    public Transform enemy;

    public bool facingRight = true;

    public LayerMask enemyLayer;



    private FighterStateFactory states;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        states = new FighterStateFactory(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentState = states.Idle();
        CurrentState.EnterState();
    }

    public void GotHit() 
    { 
        CurrentState.SwitchState(states.Hit()); 
    }


    // Update is called once per frame
    void Update()
    {
        UpdateFacing();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Reducir cooldown del dash
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;


        if (!isDummy)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            jumpPressed = Input.GetButtonDown("Jump");
            fastFallPressed = Input.GetKey(KeyCode.S);

            lightPressed = Input.GetKeyDown(KeyCode.J);
            heavyPressed = Input.GetKeyDown(KeyCode.K);

            FacingDirection inputDir = FacingDirection.Neutral;

            if (Input.GetKeyDown(KeyCode.D))
                inputDir = FacingDirection.Right;

            if (Input.GetKeyDown(KeyCode.A))
                inputDir = FacingDirection.Left;

            bool isForward = false;
            bool isBack = false;

            if (inputDir != FacingDirection.Neutral)
            {
                if (facingRight)
                {
                    if (inputDir.Equals( FacingDirection.Right))
                        isForward = true;
                    else
                        isBack = true;
                }
                else
                {
                    if (inputDir.Equals (FacingDirection.Left))
                        isForward = true;
                    else
                        isBack = true;
                }
            }

            if (isForward)
            {
                if (Time.time - lastForwardTap < doubleTapThreshold && dashCooldownTimer <= 0f)
                {
                    dashDirection = +1;
                    CurrentState.SwitchState(states.Dash());
                }
                lastForwardTap = Time.time;
            }


            if (isBack)
            {
                if (Time.time - lastBackTap < doubleTapThreshold && dashCooldownTimer <= 0f)
                {
                    dashDirection = -1;
                    CurrentState.SwitchState(states.Dash());
                }
                lastBackTap = Time.time;
            }

        }
        else
        {
            if (!(CurrentState is FighterBlockState))
                CurrentState.SwitchState(states.Block());

            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            CurrentState.UpdateState();
            return;
        }

        if (Input.GetKey(KeyCode.B) && !isDummy)
        {
            FighterHealth healthh = GetComponent<FighterHealth>();

            if (healthh.currentBlock > 0)
            {
                if (!(CurrentState is FighterBlockState))
                    CurrentState.SwitchState(states.Block());

            }
            else
            {
                if (CurrentState is FighterBlockState)
                    CurrentState.SwitchState(states.Idle());
            }
        }
        else
        {
            if (CurrentState is FighterBlockState)
                CurrentState.SwitchState(states.Idle());
        }
        FighterHealth health = GetComponent<FighterHealth>();

        if (Input.GetKey(KeyCode.B) && health.currentBlock > 0 && health.blockCooldownTimer <= 0)
        {
            if (!(CurrentState is FighterBlockState))
                CurrentState.SwitchState(states.Block());
        }
        else
        {
            if (CurrentState is FighterBlockState)
                CurrentState.SwitchState(states.Idle());
        }



        CurrentState.UpdateState();
    }


    public void UpdateFacing()
    {
        if (enemy == null) return;

        if (enemy.position.x > transform.position.x)
            facingRight = true;
        else
            facingRight = false;

        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x); //el ? es un if, significa que si facingright scale x sino scale -x
        transform.localScale = scale;
    }

    void OnDrawGizmos()
    {
        if (CurrentState == null)
            return;

        if (CurrentState is FighterLightComboState light)
        {
            AttackData atk = light.GetCurrentAttack();
            float t = light.GetTimer();

            float activeStart = atk.startup;
            float activeEnd = atk.startup + atk.active;

            if (t < activeStart || t > activeEnd)
                return;

            float dir = facingRight ? 1f : -1f;

            Vector2 center = (Vector2)transform.position +
                             new Vector2(atk.hitboxOffset.x * dir, atk.hitboxOffset.y);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center, atk.hitboxSize);
        }
        if (CurrentState is FighterHeavyComboState heavy)
        {
            AttackData atk = heavy.GetCurrentAttack();
            float t = heavy.GetTimer();

            float activeStart = atk.startup;
            float activeEnd = atk.startup + atk.active;

            if (t < activeStart || t > activeEnd)
                return;

            float dir = facingRight ? 1f : -1f;

            Vector2 center = (Vector2)transform.position +
                             new Vector2(atk.hitboxOffset.x * dir, atk.hitboxOffset.y);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center, atk.hitboxSize);
        }
    }



}