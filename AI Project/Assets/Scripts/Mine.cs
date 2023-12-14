using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public float explosionRadius = 5f;
    public int damage = 50;

    private bool exploded = false;

    void OnTriggerEnter(Collider other)
    {
        if (!exploded && other.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    void Explode()
    {
        exploded = true;

        // Play explosion sound or visual effects

        // Detect enemies in the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                // Deal damage to enemies
                //EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                //if (enemyHealth != null)
                //{
                //    enemyHealth.TakeDamage(damage);
                //}
            }
        }

        // Destroy the mine after the explosion
        Destroy(gameObject);
    }
}
