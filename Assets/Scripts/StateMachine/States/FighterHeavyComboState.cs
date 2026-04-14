using UnityEngine;

public class FighterHeavyComboState : FighterBaseState
{
    public AttackData[] attacks;

    private int step;        
    private float timer;     
    private bool hitDone;

    public FighterHeavyComboState(FighterStateMachine ctx, FighterStateFactory factory)
       : base(ctx, factory) { }

    public override void EnterState()
    {
        ctx.rb.linearVelocity = new Vector2(0f, ctx.rb.linearVelocity.y);

        attacks = ctx.heavyComboAttacks;
        step = 0;
        timer = 0f;
        hitDone = false;
        // ctx.animator.Play(attacks[step].animationName);
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;

        AttackData atk = attacks[step];

        float totalDuration = atk.startup + atk.active + atk.recovery;
        float activeStart = atk.startup;
        float activeEnd = atk.startup + atk.active;
        if (timer >= activeStart && timer <= activeEnd)
        {
            if (!hitDone)
                DoHitbox(atk);
        }


        if (step < attacks.Length - 1) 
        {
            if (ctx.heavyPressed &&
                timer >= atk.cancelStart &&
                timer <= atk.cancelEnd)
            {
                GoToNextStep();
                return;
            }
        }

        if (timer >= totalDuration)
        {
            if (Mathf.Abs(ctx.horizontalInput) > 0.1f)
                SwitchState(factory.Run());
            else
                SwitchState(factory.Idle());
        }
    }

    private void GoToNextStep()
    {
        step++;
        timer = 0f;
        hitDone = false;


        // ctx.animator.Play(attacks[step].animationName);
    }

    private void DoHitbox(AttackData atk)
    {
        float dir = ctx.facingRight ? 1f : -1f; //significa que si facingright es true dir es 1 sino dir es -1

        Vector2 center = (Vector2)ctx.transform.position +
                         new Vector2(atk.hitboxOffset.x * dir, atk.hitboxOffset.y);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, atk.hitboxSize, 0f, ctx.enemyLayer);

        foreach (var h in hits)
        {
            FighterHealth hp = h.GetComponent<FighterHealth>();
            if (hp != null)
            {
                hp.TakeDamage(atk);
                hitDone = true;
            }

             Debug.Log("Golpe Heavy a: " + h.name);
        }

    }

    public override void ExitState()
    {
    }

    public AttackData GetCurrentAttack()
    {
        return attacks[step];
    }

    public float GetTimer()
    {
        return timer;
    }
}
