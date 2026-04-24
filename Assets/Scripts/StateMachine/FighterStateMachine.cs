using UnityEngine;

public class FighterStateMachine : MonoBehaviour
{
    public FighterBaseState CurrentState { get; set; }

    public BasicMovementDatas basicMovementDatas;

    public bool isDummy = false;
    public bool isPlayer1 = true;

    public Animator animator;
    public string animationPrefix = "naruto";

    enum FacingDirection
    {
        Left = -1,
        Right = 1,
        Neutral = 0
    }

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;

    public Rigidbody2D rb;
    public float horizontalInput;
    public bool fastFallPressed;
    public bool jumpPressed;
    public bool isGrounded;

    public bool lightPressed;
    public bool heavyPressed;

    private bool blockPressed;
    private bool forwardKeyDown;
    private bool backKeyDown;

    public AttackData[] lightComboAttacks;
    public AttackData[] heavyComboAttacks;

    public float dashCooldown = 0.4f;
    public float dashCooldownTimer = 0f;

    public int dashDirection;
    public float lastForwardTap;
    public float lastBackTap;

    public float doubleTapThreshold = 0.25f;
    public bool airDashUsed;

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

    void Start()
    {
        if (isPlayer1)
            enemyLayer = LayerMask.GetMask("Player2");
        else
            enemyLayer = LayerMask.GetMask("Player1");

        CurrentState = states.Idle();
        CurrentState.EnterState();
    }


    public void GotHit()
    {
        CurrentState.SwitchState(states.Hit());
    }

    void Update()
    {
        UpdateFacing();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (!isDummy)
        {
            if (isPlayer1)
            {
                if (Input.GetKey(KeyCode.D))
                {
                    horizontalInput = 1;
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    horizontalInput = -1;
                }
                else
                {
                    horizontalInput = 0;
                }

                jumpPressed = Input.GetKeyDown(KeyCode.W);
                fastFallPressed = Input.GetKey(KeyCode.S);

                lightPressed = Input.GetKeyDown(KeyCode.J);
                heavyPressed = Input.GetKeyDown(KeyCode.K);

                blockPressed = Input.GetKey(KeyCode.B);

                forwardKeyDown = Input.GetKeyDown(KeyCode.D);
                backKeyDown = Input.GetKeyDown(KeyCode.A);
            }
            else
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    horizontalInput = 1;
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    horizontalInput = -1;
                }
                else
                {
                    horizontalInput = 0;
                }

                jumpPressed = Input.GetKeyDown(KeyCode.UpArrow);
                fastFallPressed = Input.GetKey(KeyCode.DownArrow);

                lightPressed = Input.GetKeyDown(KeyCode.N);
                heavyPressed = Input.GetKeyDown(KeyCode.M);

                blockPressed = Input.GetKey(KeyCode.Period);

                forwardKeyDown = Input.GetKeyDown(KeyCode.RightArrow);
                backKeyDown = Input.GetKeyDown(KeyCode.LeftArrow);
            }

            FacingDirection inputDir = FacingDirection.Neutral;

            if (forwardKeyDown)
            {
                inputDir = FacingDirection.Right;
            }

            if (backKeyDown)
            {
                inputDir = FacingDirection.Left;
            }

            bool isForward = false;
            bool isBack = false;

            if (inputDir != FacingDirection.Neutral)
            {
                if (facingRight)
                {
                    if (inputDir == FacingDirection.Right)
                    {
                        isForward = true;
                    }
                    else
                    {
                        isBack = true;
                    }
                }
                else
                {
                    if (inputDir == FacingDirection.Left)
                    {
                        isForward = true;
                    }
                    else
                    {
                        isBack = true;
                    }
                }
            }

            if (isForward)
            {
                if (Time.time - lastForwardTap < doubleTapThreshold && dashCooldownTimer <= 0f)
                {
                    dashDirection = 1;
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

            FighterHealth health = GetComponent<FighterHealth>();

            if (blockPressed)
            {
                if (health.currentBlock > 0 && isGrounded && health.blockCooldownTimer <= 0)
                {
                    if (!(CurrentState is FighterBlockState))
                    {
                        CurrentState.SwitchState(states.Block());
                    }
                }
                else
                {
                    if (CurrentState is FighterBlockState)
                    {
                        CurrentState.SwitchState(states.Idle());
                    }
                }
            }
            else
            {
                if (CurrentState is FighterBlockState)
                {
                    CurrentState.SwitchState(states.Idle());
                }
            }
        }
        else
        {
            if (!(CurrentState is FighterBlockState))
            {
                CurrentState.SwitchState(states.Block());
            }

            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            CurrentState.UpdateState();
            return;
        }

        CurrentState.UpdateState();
    }

    public void UpdateFacing()
    {
        if (enemy == null)
        {
            return;
        }

        if (enemy.position.x > transform.position.x)
        {
            facingRight = true;
        }
        else
        {
            facingRight = false;
        }

        Vector3 scale = transform.localScale;

        if (facingRight)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }
}
