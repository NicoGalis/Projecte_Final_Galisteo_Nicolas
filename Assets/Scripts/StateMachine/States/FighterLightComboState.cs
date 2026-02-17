using UnityEngine;

public class FighterLightComboState : FighterBaseState
{
    // Lista de ataques (Light1, Light2, Light3)
    public AttackData[] attacks;

    private int step;        // 0 = primer golpe, 1 = segundo, 2 = tercero
    private float timer;     // tiempo dentro del golpe actual
    private bool hitDone;    // para no golpear dos veces en el mismo golpe


    public FighterLightComboState(FighterStateMachine ctx, FighterStateFactory factory)
        : base(ctx, factory) { }

    public override void EnterState()
    {
    
        attacks = ctx.lightComboAttacks;
        step = 0;
        timer = 0f;
        hitDone = false;
        // Aquí podrías reproducir animación:
        // ctx.animator.Play(attacks[step].animationName);
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;

        AttackData atk = attacks[step];

        float totalDuration = atk.startup + atk.active + atk.recovery;
        float activeStart = atk.startup;
        float activeEnd = atk.startup + atk.active;

        // 1) HITBOX: solo durante la ventana activa
        if (!hitDone && timer >= activeStart && timer <= activeEnd)
        {
            DoHitbox(atk);
            hitDone = true;
        }

        // 2) CANCEL: si pulsas light dentro de la ventana, pasas al siguiente golpe
        if (step < attacks.Length - 1) // si no es el último golpe
        {
            if (ctx.lightPressed &&
                timer >= atk.cancelStart &&
                timer <= atk.cancelEnd)
            {
                GoToNextStep();
                return;
            }
        }

        // 3) FIN DEL GOLPE: volver a Idle o Run
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

        // Cambiar animación si quieres:
        // ctx.animator.Play(attacks[step].animationName);
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
                hp.TakeDamage(atk.damage);
            }

            Debug.Log("Golpe light a: " + h.name);
        }

    }

    public override void ExitState()
    {
        // Limpieza si hace falta
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
