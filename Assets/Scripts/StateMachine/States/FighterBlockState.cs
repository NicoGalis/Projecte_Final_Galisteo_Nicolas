using UnityEngine;

public class FighterBlockState : FighterBaseState
{
    public FighterBlockState(FighterStateMachine ctx, FighterStateFactory factory)
        : base(ctx, factory) { }


    public override void EnterState()
    {
        ctx.animator.SetBool("isBlocking", true);
        ctx.rb.linearVelocity = Vector2.zero;

        var health = ctx.GetComponent<FighterHealth>();
        if (health.currentBlock > 0)
        {
            health.currentBlock -= 50f;
            health.currentBlock = Mathf.Clamp(health.currentBlock, 0, health.basicData.Blockmeter);
            health.UpdateBlockBar();
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
