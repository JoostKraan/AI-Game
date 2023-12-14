using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    // De gezondheid van de zombie, toegankelijk vanuit andere scripts
    public int health = 100;

    void Update()
    {
        // Controleer of de gezondheid nul is
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
