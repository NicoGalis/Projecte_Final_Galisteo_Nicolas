using UnityEngine;
using UnityEngine.UI;

public class FighterRunState : FighterBaseState
{
    bool isRunning;
    public FighterRunState(FighterStateMachine ctx, FighterStateFactory factory) : base(ctx, factory) { }
    public override void EnterState()
    {
        isRunning = true;
    }
    public override void UpdateState()
    {

        ctx.UpdateFacing();
        if (!ctx.isGrounded)
        {
            SwitchState(factory.Fall());
            return;
        }
        if (Mathf.Abs(ctx.horizontalInput) < 0.1f)
        {
            SwitchState(factory.Idle());
            return;
        }

        if (ctx.jumpPressed && ctx.isGrounded)
        {
            SwitchState(factory.Jump());
            return;
        }
        Vector2 move = new Vector2(ctx.horizontalInput * ctx.basicMovementDatas.speed, ctx.rb.linearVelocity.y);
        ctx.rb.linearVelocity = move;
        
        ctx.animator.SetBool("isRunning", isRunning);


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
        isRunning = false;
        ctx.animator.SetBool("isRunning", isRunning);
    }


}
