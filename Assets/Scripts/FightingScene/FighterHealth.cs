using UnityEngine;
using UnityEngine.UI;

public class FighterHealth : MonoBehaviour
{
    public FighterBasicData basicData;   // ScriptableObject con la vida máxima
    public Image healthFill;             // Relleno de la barra de vida

    public float currentHealth;

    void Start()
    {
        currentHealth = basicData.Health;
        UpdateHealthBar();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, basicData.Health);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Debug.Log(name + " ha sido derrotado");
            // Aquí puedes poner animación de KO, freeze, etc.
        }
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
