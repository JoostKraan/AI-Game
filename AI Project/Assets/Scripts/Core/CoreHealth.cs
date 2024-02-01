using UnityEngine;
using UnityEngine.UI;

public class CoreHealth : MonoBehaviour
{
    public int coreHealth = 100;
    public Image healthBar;
    public static int coreStaticHealth;

    public GameObject lostPanel;

    private void Start()
    {
        coreStaticHealth = coreHealth;
        Time.timeScale = 1;
    }

    void Update()
    {
        TakeDamage();
    }

    public void TakeDamage()
    {
        // Update de gezondheidsbalk wanneer er schade wordt toegebracht
        UpdateHealthBar();

        if (coreStaticHealth <= 0)
        {
            Destroy(gameObject);
            lostPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    void UpdateHealthBar()
    {
        // Bereken de vullingsgraad van de gezondheidsbalk op basis van de huidige gezondheid
        float fillAmount = (float)coreStaticHealth / 100f;

        // Werk de vullingsgraad van de gezondheidsbalk bij
        healthBar.fillAmount = fillAmount;

        if (coreStaticHealth == coreHealth) healthBar.color = Color.green;
        else if(coreStaticHealth <= 50 && coreStaticHealth > 20) healthBar.color = Color.yellow;
        else if(coreStaticHealth <= 20) healthBar.color = Color.red;
    }
}
