using UnityEngine;

public class FighterLightComboState : FighterBaseState
{
    // Lista de ataques (Light1, Light2, Light3)
    public AttackData[] attacks;

    private int step;        // 0 = primer golpe, 1 = segundo, 2 = tercero
    private float timer;     // tiempo dentro del golpe actual
    private bool hitDone;    // para no golpear dos veces en el mismo golpe

    // Nombres de animación fijos, en el mismo orden que los ataques
    private readonly string[] lightAnimNames = { "Light1", "Light2", "Light3" };

    public FighterLightComboState(FighterStateMachine ctx, FighterStateFactory factory)
        : base(ctx, factory) { }

    public override void EnterState()
    {
        // Frenar movimiento horizontal al iniciar el combo
        ctx.rb.linearVelocity = new Vector2(0f, ctx.rb.linearVelocity.y);

        attacks = ctx.lightComboAttacks;
        step = 0;
        timer = 0f;
        hitDone = false;

        // Reproducir animación del primer golpe (Light1)
        PlayCurrentLightAnimation();
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;

        AttackData atk = attacks[step];

        float totalDuration = atk.startup + atk.active + atk.recovery;
        float activeStart = atk.startup;
        float activeEnd = atk.startup + atk.active;

        // Activar hitbox durante frames activos
        if (!hitDone && timer >= activeStart && timer <= activeEnd)
        {
            DoHitbox(atk);
            hitDone = true;
        }

        // Cancelar al siguiente golpe si se pulsa light dentro del cancel window
        if (step < attacks.Length - 1)
        {
            if (ctx.lightPressed &&
                timer >= atk.cancelStart &&
                timer <= atk.cancelEnd)
            {
                GoToNextStep();
                return;
            }
        }

        // Termina el golpe  volver a Idle o Run
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

        // Reproducir animación del siguiente golpe (Light2 o Light3)
        PlayCurrentLightAnimation();
    }

    private void PlayCurrentLightAnimation()
    {
        // Seguridad: por si acaso hay menos animaciones que pasos
        int index = Mathf.Clamp(step, 0, lightAnimNames.Length - 1);
        ctx.animator.Play(lightAnimNames[index], 0, 0f);
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
            }
        }
    }

    public override void ExitState()
    {
        ctx.animator.Play("narutoIdle", 0, 0f);
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
