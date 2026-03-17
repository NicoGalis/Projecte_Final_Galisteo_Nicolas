using UnityEngine;

public class FighterIdleState : FighterBaseState
{
    public FighterIdleState(FighterStateMachine ctx, FighterStateFactory factory) : base(ctx, factory) { }

    public override void EnterState()
    {
        
    }

    public override void UpdateState()
    {
        ctx.UpdateFacing();

        if (!ctx.isGrounded)
        {
            SwitchState(factory.Fall());
            return;
        }
        if (Mathf.Abs(ctx.horizontalInput) > 0.1f)
        {
            SwitchState(factory.Run());
            return;
        }

        if (ctx.jumpPressed && ctx.isGrounded)
        {
            SwitchState(factory.Jump());
            return;
        }

        // Si no hay input horizontal, frenar suavemente
        if (ctx.horizontalInput == 0)
        {
            ctx.rb.linearVelocity = new Vector2(0f, ctx.rb.linearVelocity.y);
        }

        if (ctx.lightPressed)
        {
            SwitchState(factory.LightCombo());
            return;
        }

        if (ctx.heavyPressed)
        {
            SwitchState(factory.HeavyCombo());
            return;
        }


    }

    public override void ExitState()
    {
    }

}
