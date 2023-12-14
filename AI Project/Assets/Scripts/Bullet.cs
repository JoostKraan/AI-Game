using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 10f; // Adjust this to set the lifetime of the bullet
    public int damage = 50;

    void Start()
    {
        // Destroy the bullet after a specified lifetime
        Destroy(gameObject, lifetime);
    }
    void OnCollisionEnter(Collision collision)
    {
        // Check if the object the bullet collided with has a Health script
        ZombieHealth healthScript = collision.gameObject.GetComponent<ZombieHealth>();

        // If the Health script is found, update the health value
        if (healthScript != null)
        {
            healthScript.health -= damage;
        }

        // Destroy the bullet
        Destroy(gameObject);
    }

}