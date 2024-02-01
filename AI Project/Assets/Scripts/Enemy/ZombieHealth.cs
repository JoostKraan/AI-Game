using UnityEngine;
using UnityEngine.UI;

public class ZombieHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    public Image healthBarFill;
    public int enemiesKilled = 0;

    // Event for when the zombie dies
    public event System.Action OnDeath;

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
        enemiesKilled++;
        // Trigger the OnDeath event
        Debug.Log(enemiesKilled);

        // Implement any death logic here
        Destroy(gameObject);
    }

    void Start()
    {
        // You can initialize any variables or perform setup here
    }
}