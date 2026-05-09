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

        ctx.animator.Play(ctx.animationPrefix + "Dash", 0, 0f);
        ctx.animator.SetBool("isDashing", true);

        if (ctx.airDashUsed)
            return;

        // veure si el dash es cap a la direccio que el jugador esta mirant o cap a la contraria
        if (ctx.facingRight)
            dashDir = 1;
        else
            dashDir = -1;

        if (ctx.dashDirection == -1) // si el jugador vol dashar cap a la direccio contraria a la que esta mirant
            dashDir *= -1;

        ctx.rb.linearVelocity = new Vector2(dashDir * ctx.dashSpeed, 0f);

        if (!ctx.isGrounded)
            ctx.airDashUsed = true;
    }

    public override void UpdateState()
    {
        ctx.rb.linearVelocity = new Vector2(dashDir * ctx.dashSpeed, 0f);

        timer -= Time.deltaTime; // el dash dura un temps limitat, i quan s'acaba, el jugador torna a l'estat normal

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
        ctx.animator.SetBool("isDashing", false);
        ctx.dashCooldownTimer = ctx.dashCooldown;
    }
}
