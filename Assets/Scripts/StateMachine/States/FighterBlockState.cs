using UnityEngine;

public class FighterBlockState : FighterBaseState
{
    public FighterBlockState(FighterStateMachine ctx, FighterStateFactory factory)
        : base(ctx, factory) { }

    public override void EnterState()
    {
        //ctx.animator.Play("Block"); // tu animación de bloqueo
    }

    public override void UpdateState()
    {
        // La lógica de drenaje está en FighterHealth
    }

    public override void ExitState()
    {
        // Nada especial
    }
}
