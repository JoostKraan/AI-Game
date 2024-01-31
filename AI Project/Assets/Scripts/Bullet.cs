using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 10f; // Adjust this to set the lifetime of the bullet
    public int damage = 50;
    public GameObject damageNumberPrefab;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Destroy the bullet after a specified lifetime
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.forward = rb.velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the object the bullet collided with has a ZombieHealth script
        ZombieHealth healthScript = collision.gameObject.GetComponent<ZombieHealth>();

        // If the ZombieHealth script is found, update the health value
        if (healthScript != null)
        {
            healthScript.TakeDamage(damage);
            SpawnDamageNumber(collision.contacts[0].point);
        }

        Destroy(gameObject);
    }
    private void SpawnDamageNumber(Vector3 position)
    {
        GameObject damageNumber = Instantiate(damageNumberPrefab, position, Quaternion.identity);
        DamageNumber damageNumberScript = damageNumber.GetComponentInChildren<DamageNumber>();
        
        if (damageNumberScript != null)
        {
            damageNumberScript.SetDamageText(damage);
        }
    }
}