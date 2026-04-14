using UnityEngine;

public class FighterDashState : FighterBaseState
{
    private float timer;
    private int dashDir;

    public FighterDashState(FighterStateMachine ctx, FighterStateFactory factory)
        : base(ctx, factory) { }

    public override void EnterState()
    {
        timer = ctx.dashDuration;

        if (ctx.airDashUsed == true)
        {
            return;
        }

        dashDir = 1;

        if (ctx.facingRight == true)
        {
            dashDir = 1;
        }
        else
        {
            dashDir = -1;
        }

        if (ctx.dashDirection == -1)
        {
            dashDir = dashDir * -1;
        }

        ctx.rb.linearVelocity = new Vector2(dashDir * ctx.dashSpeed, 0f);

        if (ctx.isGrounded == false)
        {
            ctx.airDashUsed = true;
        }
    }

    public override void UpdateState()
    {
        ctx.rb.linearVelocity = new Vector2(dashDir * ctx.dashSpeed, 0f);

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (ctx.horizontalInput != 0)
            {
                SwitchState(factory.Run());
            }
            else
            {
                SwitchState(factory.Idle());
            }
            return;
        }

        if (ctx.isGrounded == true && ctx.jumpPressed == true)
        {
            SwitchState(factory.Jump());
            return;
        }
    }

    public override void ExitState()
    {
        ctx.dashCooldownTimer = ctx.dashCooldown;
    }

}
