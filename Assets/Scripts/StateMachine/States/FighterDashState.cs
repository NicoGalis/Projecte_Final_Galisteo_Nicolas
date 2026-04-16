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

        // Activar animación de dash
        ctx.animator.SetBool("isDashing", true);

        if (ctx.airDashUsed)
            return;

        dashDir = ctx.facingRight ? 1 : -1;

        if (ctx.dashDirection == -1)
            dashDir *= -1;

        ctx.rb.linearVelocity = new Vector2(dashDir * ctx.dashSpeed, 0f);

        if (!ctx.isGrounded)
            ctx.airDashUsed = true;
    }

    public override void UpdateState()
    {
        ctx.rb.linearVelocity = new Vector2(dashDir * ctx.dashSpeed, 0f);

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (ctx.horizontalInput != 0)
                SwitchState(factory.Run());
            else
                SwitchState(factory.Idle());
            return;
        }

        if (ctx.isGrounded && ctx.jumpPressed)
        {
            SwitchState(factory.Jump());
            return;
        }
    }

    public override void ExitState()
    {
        // Desactivar animación de dash
        ctx.animator.SetBool("isDashing", false);

        ctx.dashCooldownTimer = ctx.dashCooldown;
    }
}
