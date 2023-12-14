using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 10f; // Adjust this to set the lifetime of the bullet

    void Start()
    {
        // Destroy the bullet after a specified lifetime
        Destroy(gameObject, lifetime);
    }

    // You may want to handle collisions with other objects if necessary
    void OnCollisionEnter(Collision collision)
    {
        // Add collision handling logic here if needed

        // Destroy the bullet upon collision
        Destroy(gameObject);
    }
}