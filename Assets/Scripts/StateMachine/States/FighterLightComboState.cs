using UnityEngine;

public class FighterLightComboState : FighterBaseState
{
    public AttackData[] attacks;

    private int step;
    private float timer;
    private bool hitDone;

    private readonly string[] lightAnimNames = { "Light1", "Light2", "Light3" };

    public FighterLightComboState(FighterStateMachine ctx, FighterStateFactory factory)
        : base(ctx, factory) { }

    public override void EnterState() // Quan entrem a l'estat de combo, resetejem la velocitat horitzontal del jugador per evitar que es mogui durant l'animació d'atac
    {
        ctx.rb.linearVelocity = new Vector2(0f, ctx.rb.linearVelocity.y); // Reseteja la velocitat horitzontal

        attacks = ctx.lightComboAttacks;
        step = 0;
        timer = 0f;
        hitDone = false;

        PlayCurrentLightAnimation();
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;

        AttackData atk = attacks[step];

        float totalDuration = atk.startup + atk.active + atk.recovery; // Calculem la durada total de l'atac sumant les fases de startup, active i recovery
        float activeStart = atk.startup; // El moment en què comença la fase activa de l'atac
        float activeEnd = atk.startup + atk.active; // El moment en què acaba la fase activa de l'atac

        if (!hitDone && timer >= activeStart && timer <= activeEnd) // Si encara no hem aplicat el hitbox i estem dins de la fase activa, apliquem el hitbox
        {
            DoHitbox(atk);
            hitDone = true;
        }

        if (step < attacks.Length - 1) // Si encara no hem arribat a l'últim atac del combo, comprovem si el jugador ha premut el botó de light per cancel·lar cap al següent atac
        {
            if (ctx.lightPressed &&
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

    private void GoToNextStep() // Quan el jugador prem el botó de light dins de la finestra de cancel·lació, avancem al següent atac del combo
    {
        step++;
        timer = 0f;
        hitDone = false;

        PlayCurrentLightAnimation();
    }

    private void PlayCurrentLightAnimation()
    {
        int index = Mathf.Clamp(step, 0, lightAnimNames.Length - 1);

        string animName = ctx.animationPrefix + lightAnimNames[index];
        ctx.animator.Play(animName, 0, 0f);
    }

    private void DoHitbox(AttackData atk)
    {
        float dir = ctx.facingRight ? 1f : -1f;

        Vector2 center = (Vector2)ctx.transform.position +
                         new Vector2(atk.hitboxOffset.x * dir, atk.hitboxOffset.y);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, atk.hitboxSize, 0f, ctx.enemyLayer);

        FighterHealth selfHealth = ctx.GetComponent<FighterHealth>();

        foreach (var h in hits)
        {
            FighterHealth hp = h.GetComponent<FighterHealth>();

            if (hp == selfHealth)
                continue;

            if (hp != null)
            {
                hp.TakeDamage(atk);
                hitDone = true;

                int id = selfHealth.characterID;

                GameManager gm = Object.FindFirstObjectByType<GameManager>();
                ctx.StartCoroutine(gm.AddLight(id));
            }

            Debug.Log("Golpe Light a: " + h.name);
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
