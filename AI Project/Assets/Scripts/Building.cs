using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health of the building
    private int currentHealth;   // Current health of the building

    protected virtual void Start()
    {
        currentHealth = maxHealth; // Initialize current health to maximum health
    }

    void Update()
    {
        // Check if the building should be destroyed
        if (currentHealth <= 0)
        {
            DestroyBuilding();
        }
    }

    // Method to apply damage to the building
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        // Ensure health doesn't go below 0
        currentHealth = Mathf.Max(currentHealth, 0);
    }

    // Method to destroy the building
    private void DestroyBuilding()
    {
        Debug.Log("Building destroyed!");
        Destroy(gameObject); // Destroy the GameObject attached to this script
    }

    // Method to change the health amount
    public void ChangeHealth(int newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
    }
}
