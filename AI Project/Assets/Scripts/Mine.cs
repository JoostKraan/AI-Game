using System.Collections;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public float explosionRadius = 5f;
    public int damage = 50;
    public float delay = 2f; // Delay before the mine explodes
    public MeshRenderer mineMeshRenderer;
    public AudioSource beepSound;
    public ParticleSystem explosionParticlesPrefab;
    public float explosionForce = 1000f; // Adjust the force as needed

    public AudioClip regularBeepClip;
    public AudioClip fastBeepClip;
    public AudioClip explosionClip;

    private bool exploded = false;

    void Start()
    {
        // Disable the mesh renderer initially
        if (mineMeshRenderer != null)
        {
            mineMeshRenderer.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Trigger the explosion immediately if an enemy enters the trigger zone
        if (!exploded && other.CompareTag("Zombie"))
        {
            CancelInvoke(); // Cancel the scheduled explosion
            Invoke(nameof(Explode), delay);
            StartCoroutine(BlinkMesh());
        }
    }

    IEnumerator BlinkMesh()
    {
        float blinkInterval = 0.2f; // Initial blink interval
        float timeToExplode = delay;

        while (!exploded && timeToExplode > 0)
        {
            if (mineMeshRenderer != null)
            {
                mineMeshRenderer.enabled = !mineMeshRenderer.enabled;
            }

            if (beepSound != null)
            {
                if (timeToExplode <= 0.5f)
                {
                    beepSound.clip = fastBeepClip;
                }
                else
                {
                    beepSound.clip = regularBeepClip;
                }

                beepSound.Play();
            }

            yield return new WaitForSeconds(blinkInterval);

            // Adjust the blink interval dynamically based on time left to explode
            timeToExplode -= blinkInterval;

            // Make the last 500ms of the beep go really fast
            if (timeToExplode <= 0.5f)
            {
                blinkInterval = Mathf.Clamp(blinkInterval * 0.8f, 0.05f, 0.1f); // Increase speed during the last 500ms
            }
            else
            {
                blinkInterval = Mathf.Clamp(blinkInterval * 0.9f, 0.1f, 1.0f); // Regular speed before the last 500ms
            }
        }
    }

    void Explode()
    {
        if (exploded) return; // Ensure the mine only explodes once
        exploded = true;

        // Stop blinking and hide the mesh
        if (mineMeshRenderer != null)
        {
            mineMeshRenderer.enabled = false;
        }

        // Play final explosion sound
        if (beepSound != null)
        {
            beepSound.clip = explosionClip;
            beepSound.Play();
        }

        // Instantiate and play explosion particle effect
        if (explosionParticlesPrefab != null)
        {
            ParticleSystem explosionParticles = Instantiate(explosionParticlesPrefab, transform.position, Quaternion.identity);
            explosionParticles.Play();
        }

        // Apply explosive force
        ApplyExplosiveForce();

        // Detect enemies in the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Zombie"))
            {
                // Deal damage to enemies
                ZombieHealth enemyHealth = hit.GetComponent<ZombieHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.health -= damage;
                }
            }
        }

        // Destroy the mine after the explosion
        Destroy(gameObject);
    }

    void ApplyExplosiveForce()
    {
        // Get all colliders in the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            // Check if the object has a Rigidbody
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply explosive force
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 3.0f);
            }
        }
    }
}
