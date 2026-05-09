using UnityEngine;

public class FighterJumpState : FighterBaseState
{
    public FighterJumpState(FighterStateMachine ctx, FighterStateFactory factory) : base(ctx, factory) { }
    private float jumpStartTime;
    private float fastFallDelay = 0.2f; 
    public override void EnterState()
    {
        jumpStartTime = Time.time;

        ctx.animator.SetBool("isJumping", true);

        Vector2 v = ctx.rb.linearVelocity; // Mantenim la velocitat horitzontal actual del jugador
        v.y = ctx.basicMovementDatas.jumpForce; // Assignem la força de salt a la component vertical
        ctx.rb.linearVelocity = v;

    }

    public override void UpdateState()
    {
        ctx.UpdateFacing();

        float airSpeed = ctx.basicMovementDatas.speed / ctx.basicMovementDatas.onAirSpeedDivisor; // Calcular velocitat horitzontal a l'aire

        if (ctx.fastFallPressed && Time.time - jumpStartTime > fastFallDelay)
        {
            ctx.rb.linearVelocity = new Vector2(ctx.rb.linearVelocity.x, -ctx.basicMovementDatas.fallSpeed * 1.0001f); // Aplicar fast fall
            SwitchState(factory.Fall());
            return;
        }

        ctx.rb.linearVelocity = new Vector2(
            ctx.horizontalInput * airSpeed,
            ctx.rb.linearVelocity.y
        );


        if (ctx.rb.linearVelocity.y <= 0) // Si el jugador està caient (velocitat vertical negativa o zero), canvia a l'estat de caiguda
        {
            SwitchState(factory.Fall());
        }

    }


    public override void ExitState()
    {
        ctx.animator.SetBool("isJumping", false);
    }

}
