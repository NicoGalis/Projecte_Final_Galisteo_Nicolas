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

    public float blockCooldown = 1.5f;          
    [HideInInspector] public float blockCooldownTimer = 0f;

    public GameManager gm;
    FighterStateMachine fsm;

    void Start() //Inicialitzacio de la vida, blockmeter i referencies
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

    void HandleBlockMeter() //Gestio del blockmeter, drain i regen
    {
        if (blockCooldownTimer > 0)
        {
            blockCooldownTimer -= Time.deltaTime; //Reduccio del timer de cooldown
            return;
        }

        if (fsm != null && fsm.CurrentState is FighterBlockState && currentBlock > 0) //Si esta bloquejant i encara te blockmeter, el drain es continua
        {
            currentBlock -= blockDrainPerSecond * Time.deltaTime;
            currentBlock = Mathf.Clamp(currentBlock, 0, basicData.Blockmeter);

            if (currentBlock <= 0)
                blockCooldownTimer = blockCooldown;

            return;
        }

        if (currentBlock < basicData.Blockmeter)//Si no esta bloquejant i no te el blockmeter al maxim, el regen es continua
        {
            currentBlock += blockRegenPerSecond * Time.deltaTime;
            currentBlock = Mathf.Clamp(currentBlock, 0, basicData.Blockmeter);
        }
    }

    public void TakeDamage(AttackData atk) //Gestio del que passa quan el personatge rep un atac, si esta bloquejant es redueix el blockmeter, sino es redueix la vida i es comprova si ha mort o no
    {
        if (fsm != null && fsm.CurrentState is FighterBlockState && currentBlock > 0 && blockCooldownTimer <= 0)
        {
            currentBlock -= atk.blockDamage;   
            currentBlock = Mathf.Clamp(currentBlock, 0, basicData.Blockmeter);

            if (currentBlock <= 0)
                blockCooldownTimer = blockCooldown;

            UpdateBlockBar();
            return;
        }


        currentHealth -= atk.damage; //Reduccio de la vida
        currentHealth = Mathf.Clamp(currentHealth, 0, basicData.Health); //Comprovacio que la vida no sigui negativa
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
