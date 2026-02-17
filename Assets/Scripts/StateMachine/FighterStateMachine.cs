using UnityEngine;

public class FighterStateMachine : MonoBehaviour
{
    public FighterBaseState CurrentState { get; set; }

    public BasicMovementDatas basicMovementDatas;

    public bool isDummy = false;


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
        states = new FighterStateFactory(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentState = states.Idle();
        CurrentState.EnterState();
    }


    // Update is called once per frame
    void Update()
    {
        // SIEMPRE mirar al enemigo (player y dummy)
        UpdateFacing();

        // SIEMPRE comprobar si est� en el suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // El dummy NO lee inputs ni se mueve
        if (!isDummy)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            jumpPressed = Input.GetButtonDown("Jump");
            fastFallPressed = Input.GetKey(KeyCode.S);

            lightPressed = Input.GetKeyDown(KeyCode.J);
            heavyPressed = Input.GetKeyDown(KeyCode.K);

            int inputDir = 0;

            if (Input.GetKeyDown(KeyCode.D))
                inputDir = 1;

            if (Input.GetKeyDown(KeyCode.A))
                inputDir = -1;

            bool isForward = false;
            bool isBack = false;

            if (inputDir != 0)
            {
                if (facingRight)
                {
                    if (inputDir == 1)
                        isForward = true;
                    else
                        isBack = true;
                }
                else
                {
                    if (inputDir == -1)
                        isForward = true;
                    else
                        isBack = true;
                }
            }

            if (isForward)
            {
                if (Time.time - lastForwardTap < doubleTapThreshold)
                {
                    dashDirection = +1;
                    CurrentState.SwitchState(states.Dash());
                }
                lastForwardTap = Time.time;
            }

            if (isBack)
            {
                if (Time.time - lastBackTap < doubleTapThreshold)
                {
                    dashDirection = -1;
                    CurrentState.SwitchState(states.Dash());
                }
                lastBackTap = Time.time;
            }
        }
        else
        {
            // Dummy NO se mueve
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // La StateMachine SIEMPRE se actualiza
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

        // �Estamos en el combo flojo?
        if (CurrentState is FighterLightComboState light)
        {
            AttackData atk = light.GetCurrentAttack();
            float t = light.GetTimer();

            // Solo dibujar durante los frames activos
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