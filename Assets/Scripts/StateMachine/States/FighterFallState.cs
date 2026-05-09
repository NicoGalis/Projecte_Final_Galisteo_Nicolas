using UnityEngine;

public class FighterFallState : FighterBaseState
{
    public FighterFallState(FighterStateMachine ctx, FighterStateFactory factory)
        : base(ctx, factory) { }

    public override void EnterState()
    {
            ctx.animator.SetBool("isFalling", true);
    }

    public override void UpdateState()
    {

        ctx.UpdateFacing();

        float airSpeed = ctx.basicMovementDatas.speed / ctx.basicMovementDatas.onAirSpeedDivisor; // Calcular velocitat horitzontal a l'aire

        float newX = ctx.horizontalInput * airSpeed;
        float newY = ctx.rb.linearVelocity.y;

        if (ctx.fastFallPressed) // Si el jugador apreta el botó de fast fall, augmenta la velocitat de caiguda
            newY = -ctx.basicMovementDatas.fallSpeed * 1.0001f;

        if (newY < -ctx.basicMovementDatas.fallSpeed) // Limitar la velocitat de caiguda a fallSpeed
            newY = -ctx.basicMovementDatas.fallSpeed;

        ctx.rb.linearVelocity = new Vector2(newX, newY);

        if (ctx.isGrounded)
            SwitchState(factory.Land());
    }

    public override void ExitState() 
    { 
            ctx.animator.SetBool("isFalling", false);
    }
}
