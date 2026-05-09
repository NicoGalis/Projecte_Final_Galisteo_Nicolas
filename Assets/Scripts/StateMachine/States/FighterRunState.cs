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
        Vector2 move = new Vector2(ctx.horizontalInput * ctx.basicMovementDatas.speed, ctx.rb.linearVelocity.y); // Calcular la velocitat horitzontal basada en l'input i la velocitat vertical actual
        ctx.rb.linearVelocity = move;
        
        ctx.animator.SetBool("isRunning", isRunning);

        float speed = ctx.basicMovementDatas.speed;

        bool movingBackwards =
            (ctx.facingRight && ctx.horizontalInput < 0) ||
            (!ctx.facingRight && ctx.horizontalInput > 0); // Comprovar si el jugador se está moviendo en la dirección opuesta a la que está mirando

        if (movingBackwards)
            speed *= 0.3f; 

     move = new Vector2(ctx.horizontalInput * speed, ctx.rb.linearVelocity.y); // Recalcular la velocidad horizontal con la reducción aplicada si se está moviendo hacia atrás
        ctx.rb.linearVelocity = move;

        ctx.animator.SetBool("isRunning", !movingBackwards);
        ctx.animator.SetBool("isBackwards", movingBackwards);

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
        ctx.animator.SetBool("isBackwards", false);
    }


}
