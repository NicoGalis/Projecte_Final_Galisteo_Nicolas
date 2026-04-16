using UnityEngine;

public class FighterBlockState : FighterBaseState
{
    public FighterBlockState(FighterStateMachine ctx, FighterStateFactory factory)
        : base(ctx, factory) { }

    public override void EnterState()
    {
        ctx.animator.SetBool("isBlocking", true);
        ctx.rb.linearVelocity = Vector2.zero;
        if(ctx.isGrounded!)
        {
            
        }
    }

    public override void UpdateState()
    {
        ctx.rb.linearVelocity = Vector2.zero;
    }

    public override void ExitState()
    {
        ctx.animator.SetBool("isBlocking", false);
    }
}
