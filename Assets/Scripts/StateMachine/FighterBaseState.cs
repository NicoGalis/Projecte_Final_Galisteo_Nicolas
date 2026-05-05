using UnityEngine;

public abstract class FighterBaseState
{
    protected FighterStateMachine  ctx;
    protected FighterStateFactory factory;

    public FighterBaseState(FighterStateMachine currentContext, FighterStateFactory fighterStateFactory)
    {
        ctx = currentContext;
        factory = fighterStateFactory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();

    public void SwitchState(FighterBaseState newState)
    {
        ExitState();
        ctx.CurrentState = newState;
        newState.EnterState();
    }
}
