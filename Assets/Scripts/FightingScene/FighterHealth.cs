using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FighterHealth : MonoBehaviour
{
    public int characterID;  
    public FighterBasicData basicData;
    public Image healthFill;

    public float currentHealth;
    public GameManager gm;

    FighterStateMachine fsm;

    void Start()
    {
        gm = FindAnyObjectByType<GameManager>();

        currentHealth = basicData.Health;
        UpdateHealthBar();

        fsm = GetComponent<FighterStateMachine>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
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
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            float ratio = currentHealth / basicData.Health;
            healthFill.transform.localScale = new Vector3(ratio, 1f, 1f);
        }
    }
}
