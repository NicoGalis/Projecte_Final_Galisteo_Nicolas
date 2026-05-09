using UnityEngine;

public class FighterLandState : FighterBaseState
{
    private float timer;

    public FighterLandState(FighterStateMachine ctx, FighterStateFactory factory)
        : base(ctx, factory) { }

    public override void EnterState()
    {
        timer = ctx.basicMovementDatas.landTime;  //agafar el temps de l'animació de landing
    }

    public override void UpdateState()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            if (Mathf.Abs(ctx.horizontalInput) > 0.1f)
                SwitchState(factory.Run());
            else
                SwitchState(factory.Idle());
        }
        ctx.airDashUsed = false;
    }

    public override void ExitState() { }
}
