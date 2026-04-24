using UnityEngine;

public class FighterHeavyComboState : FighterBaseState
{
    public AttackData[] attacks;

    private int step;
    private float timer;
    private bool hitDone;

    private readonly string[] heavyAnimNames = { "Heavy1", "Heavy2", "Heavy3" };

    public FighterHeavyComboState(FighterStateMachine ctx, FighterStateFactory factory)
       : base(ctx, factory) { }

    public override void EnterState()
    {
        ctx.rb.linearVelocity = new Vector2(0f, ctx.rb.linearVelocity.y);

        attacks = ctx.heavyComboAttacks;
        step = 0;
        timer = 0f;
        hitDone = false;

        PlayCurrentHeavyAnimation();
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;

        AttackData atk = attacks[step];

        float totalDuration = atk.startup + atk.active + atk.recovery;
        float activeStart = atk.startup;
        float activeEnd = atk.startup + atk.active;

        if (!hitDone && timer >= activeStart && timer <= activeEnd)
        {
            DoHitbox(atk);
            hitDone = true;
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

        PlayCurrentHeavyAnimation();
    }

    private void PlayCurrentHeavyAnimation()
    {
        int index = Mathf.Clamp(step, 0, heavyAnimNames.Length - 1);

        string animName = ctx.animationPrefix + heavyAnimNames[index];
        ctx.animator.Play(animName, 0, 0f);
    }

    private void DoHitbox(AttackData atk)
    {
        float dir = ctx.facingRight ? 1f : -1f;

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

                int id = ctx.GetComponent<FighterHealth>().characterID;

                GameManager gm = Object.FindFirstObjectByType<GameManager>();
                ctx.StartCoroutine(gm.AddHeavy(id));
            }

            Debug.Log("Golpe Heavy a: " + h.name);
        }
    }

    public override void ExitState()
    {
        string idleAnim = ctx.animationPrefix + "Idle";
        ctx.animator.Play(idleAnim, 0, 0f);
    }

    public AttackData GetCurrentAttack() => attacks[step];
    public float GetTimer() => timer;
}
