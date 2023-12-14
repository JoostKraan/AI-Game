using UnityEngine;
using UnityEngine.UI;

public class ZombieHealth : MonoBehaviour
{
    // De gezondheid van de zombie, toegankelijk vanuit andere scripts
    public int maxHealth = 100;
    public int currentHealth = 100;
    public Image healthBarFill;
   
    

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Ensure health doesn't go below zero
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void UpdateHealthBar()
    {
        float fillAmount = (float)currentHealth / maxHealth;
        healthBarFill.fillAmount = fillAmount;
    }

    void Die()
    {
        // Implement any death logic here
        Destroy(gameObject);
    }


    void start()
    {

    }
}
