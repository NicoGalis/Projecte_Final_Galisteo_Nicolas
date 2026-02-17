using UnityEngine;

public class FighterStateFactory
{
    private FighterStateMachine ctx;

    public FighterStateFactory(FighterStateMachine currentContext)
    {
        ctx = currentContext;
    }

    public FighterBaseState Idle() => new FighterIdleState(ctx, this);
    public FighterBaseState Run() => new FighterRunState(ctx, this);
    public FighterBaseState Jump() => new FighterJumpState(ctx, this);
    public FighterBaseState Fall() => new FighterFallState(ctx, this);
    public FighterBaseState Land() => new FighterLandState(ctx, this);
    public FighterBaseState Dash() => new FighterDashState(ctx, this);
    public FighterBaseState LightCombo() => new FighterLightComboState(ctx, this);



}

