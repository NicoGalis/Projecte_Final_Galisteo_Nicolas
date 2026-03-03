using UnityEngine;
using System.Collections;

public class FighterHitState : FighterBaseState
{
    private float hitstun = 0.12f;
    private float timer = 0f;

    private Vector3 originalPos;  
    private bool hitstopDone = false;

    public FighterHitState(FighterStateMachine ctx, FighterStateFactory factory)
        : base(ctx, factory) { }

    public override void EnterState()
    {
        ctx.rb.linearVelocity = Vector2.zero;
        timer = 0f;
        hitstopDone = false;

        originalPos = ctx.transform.localPosition;

        ctx.StartCoroutine(HitstopRoutine(0.05f));   // 50 ms de pausa
        ctx.StartCoroutine(ShakeRoutine(0.1f, 0.1f)); // 0.1s de shake suave

        Debug.Log("Entered Hit State");
    }

    public override void UpdateState()
    {
        if (!hitstopDone)
            return;

        timer += Time.deltaTime;

        if (timer >= hitstun)
        {
            SwitchState(factory.Idle());
        }
    }

    public override void ExitState()
    {
        //en el cas de que el shake quedi a mitjes, asegurem la posicio inicial
        ctx.transform.localPosition = originalPos;
    }

    private IEnumerator HitstopRoutine(float duration)
    {
        float originalTimeScale = Time.timeScale;

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = originalTimeScale;

        hitstopDone = true;
    }


    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            ctx.transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        ctx.transform.localPosition = originalPos;
    }
}
