using UnityEngine;
using UnityEngine.UI;

public class FighterHealth : MonoBehaviour
{
    public int characterID;
    public FighterBasicData basicData;

    public Image healthFill;
    public Image blockFill;

    public float currentHealth;
    public float currentBlock;

    public float blockDrainPerHit = 25f;
    public float blockDrainPerSecond = 20f;
    public float blockRegenPerSecond = 10f;

    public float blockCooldown = 1.5f;          // Tiempo sin poder bloquear
    [HideInInspector] public float blockCooldownTimer = 0f;

    public GameManager gm;
    FighterStateMachine fsm;

    void Start()
    {
        gm = FindAnyObjectByType<GameManager>();

        currentHealth = basicData.Health;
        currentBlock = basicData.Blockmeter;

        UpdateHealthBar();
        UpdateBlockBar();

        fsm = GetComponent<FighterStateMachine>();
    }

    void Update()
    {
        HandleBlockMeter();
        UpdateBlockBar();
    }

    void HandleBlockMeter()
    {
        // Si está en cooldown  no bloquear ni regenerar
        if (blockCooldownTimer > 0)
        {
            blockCooldownTimer -= Time.deltaTime;
            return;
        }

        // Si estás bloqueando drenar
        if (fsm != null && fsm.CurrentState is FighterBlockState && currentBlock > 0)
        {
            currentBlock -= blockDrainPerSecond * Time.deltaTime;
            currentBlock = Mathf.Clamp(currentBlock, 0, basicData.Blockmeter);

            // Si se quedó sin barra  activar cooldown
            if (currentBlock <= 0)
                blockCooldownTimer = blockCooldown;

            return;
        }

        // Regenerar cuando NO estás bloqueando
        if (currentBlock < basicData.Blockmeter)
        {
            currentBlock += blockRegenPerSecond * Time.deltaTime;
            currentBlock = Mathf.Clamp(currentBlock, 0, basicData.Blockmeter);
        }
    }

    public void TakeDamage(AttackData atk)
    {
        // Si estás bloqueando y tienes barra  NO recibes daño
        if (fsm != null && fsm.CurrentState is FighterBlockState && currentBlock > 0 && blockCooldownTimer <= 0)
        {
            currentBlock -= atk.blockDamage;   //AHORA USA EL VALOR DEL ATAQUE
            currentBlock = Mathf.Clamp(currentBlock, 0, basicData.Blockmeter);

            if (currentBlock <= 0)
                blockCooldownTimer = blockCooldown;

            UpdateBlockBar();
            return;
        }


        // Daño normal
        currentHealth -= atk.damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, basicData.Health);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            gm.PlayerDied(this);
        }
        else
        {
            if (fsm != null)
                fsm.GotHit();
        }
    }

    public void ResetHealth()
    {
        currentHealth = basicData.Health;
        currentBlock = basicData.Blockmeter;
        blockCooldownTimer = 0f;

        UpdateHealthBar();
        UpdateBlockBar();
    }

    void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            float ratio = currentHealth / basicData.Health;
            ratio = Mathf.Clamp01(ratio);
            healthFill.transform.localScale = new Vector3(ratio, 1f, 1f);
        }
    }

    void UpdateBlockBar()
    {
        if (blockFill != null)
        {
            float ratio = currentBlock / basicData.Blockmeter;
            ratio = Mathf.Clamp01(ratio);
            blockFill.transform.localScale = new Vector3(ratio, 1f, 1f);
        }
    }
}
