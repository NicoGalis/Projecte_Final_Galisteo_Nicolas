using UnityEngine;

public class FighterFallState : FighterBaseState
{
    public FighterFallState(FighterStateMachine ctx, FighterStateFactory factory)
        : base(ctx, factory) { }

    public override void EnterState()
    {
    }

    public override void UpdateState()
    {

        ctx.UpdateFacing();

        float airSpeed = ctx.basicMovementDatas.speed / ctx.basicMovementDatas.onAirSpeedDivisor;

        float newX = ctx.horizontalInput * airSpeed;
        float newY = ctx.rb.linearVelocity.y;

        if (ctx.fastFallPressed)
            newY = -ctx.basicMovementDatas.fallSpeed * 1.0001f;

        if (newY < -ctx.basicMovementDatas.fallSpeed)
            newY = -ctx.basicMovementDatas.fallSpeed;

        ctx.rb.linearVelocity = new Vector2(newX, newY);

        if (ctx.isGrounded)
            SwitchState(factory.Land());
    }

    public override void ExitState() { }
}
