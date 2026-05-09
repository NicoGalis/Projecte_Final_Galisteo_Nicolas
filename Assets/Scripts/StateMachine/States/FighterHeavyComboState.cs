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

    public override void EnterState() // Quan entrem a l'estat de heavy combo, inicialitzem les variables i reproduim la primera animació del combo
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
        timer += Time.deltaTime; // Incrementem el timer per controlar les diferents fases de l'atac

        AttackData atk = attacks[step]; //  Obtenim les dades de l'atac actual

        float totalDuration = atk.startup + atk.active + atk.recovery; // Calculem la durada total de l'atac sumant les fases de startup, active i recovery
        float activeStart = atk.startup; // El moment en què comença la fase activa de l'atac
        float activeEnd = atk.startup + atk.active; // El moment en què acaba la fase activa de l'atac

        if (!hitDone && timer >= activeStart && timer <= activeEnd) // Si encara no hem aplicat el hitbox i estem dins de la fase activa, apliquem el hitbox
        {
            DoHitbox(atk);
            hitDone = true;
        }

        if (step < attacks.Length - 1) // Si encara no hem arribat a l'últim atac del combo, comprovem si el jugador ha premut el botó de heavy per cancel·lar cap al següent atac
        {
            if (ctx.heavyPressed &&
                timer >= atk.cancelStart &&
                timer <= atk.cancelEnd)
            {
                GoToNextStep();
                return;
            }
        }

        if (timer >= totalDuration) // Si ja ha passat la durada total de l'atac, tornem a l'estat de idle o run segons si el jugador està movent-se o no
        {
            if (Mathf.Abs(ctx.horizontalInput) > 0.1f)
                SwitchState(factory.Run());
            else
                SwitchState(factory.Idle());
        }
    }

    private void GoToNextStep() // Quan el jugador prem el botó de heavy dins de la finestra de cancel·lació, avancem al següent atac del combo
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
        float dir;

        if (ctx.facingRight)
            dir = 1f;
        else
            dir = -1f;

        Vector2 center = (Vector2)ctx.transform.position +
                         new Vector2(atk.hitboxOffset.x * dir, atk.hitboxOffset.y); // Calculem el centre del hitbox sumant la posició del jugador amb l'offset del hitbox, tenint en compte la direcció a la que està mirant el jugador

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, atk.hitboxSize, 0f, ctx.enemyLayer); // Obtenim tots els col·liders que intersecten amb el hitbox, utilitzant la capa d'enemics per filtrar només els col·liders dels enemics

        foreach (var h in hits) // Per cada col·lider que ha intersectat amb el hitbox, apliquem el dany corresponent
        {
            FighterHealth hp = h.GetComponent<FighterHealth>();
            if (hp != null)
            {
                hp.TakeDamage(atk); // Apliquem el dany a l'enemic utilitzant les dades de l'atac
                hitDone = true;

                int id = ctx.GetComponent<FighterHealth>().characterID; // Obtenim l'ID del jugador que ha realitzat l'atac per passar-lo al GameManager i actualitzar les estadístiques

                GameManager gm = Object.FindFirstObjectByType<GameManager>();
                ctx.StartCoroutine(gm.AddHeavy(id));
            }

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
