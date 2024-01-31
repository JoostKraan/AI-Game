using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Building
{
    public float rotationSpeed = 5f;
    public float shootingCooldown = 1f;
    public float shootingRange = 10f;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletForce = 10f;
    public AudioClip shootSound;
    public ParticleSystem muzzleFlash;
    public GameObject modelToRotate;

    private Transform target;
    private float lastShotTime;
    private AudioSource audioSource;

    public GameObject barrel;
    public float barrelRotationSpeed = 5f;
    public float barrelRotationAcceleration = 2f;

    public float BarrelRotationRandomness = 50f;
    public float TurretRotationRandomness = 10.0f;
    public float TurretTargetRotationThreshold = 50.0f;
    public float BarrelZRotationIncrement = 50f;

    public bool canShoot { get; set; }

    public int durability = 3; // Adjust the starting durability as needed

    protected override void Start()
    {
        base.Start();

        FindNearestEnemy();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        print(canShoot);

        if (!canShoot) return;

        if (target != null && Vector3.Distance(transform.position, target.position) <= shootingRange)
        {
            RotateTowardsTarget();

            if (IsTurretCloseToTargetRotation() && Time.time - lastShotTime >= shootingCooldown)
            {
                Shoot();

                // Decrease durability
                durability--;

                // Check if durability reached 0 and destroy the turret
                if (durability <= 0)
                {
                    DestroyTurret();
                }
            }
        }
        else
        {
            FindNearestEnemy();
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = target.position - modelToRotate.transform.position;
        direction.y = 0f;

        if (Vector3.Distance(modelToRotate.transform.position, target.position) <= shootingRange)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            float randomAngle = Random.Range(-TurretRotationRandomness, TurretRotationRandomness);
            toRotation *= Quaternion.Euler(0, randomAngle + 180f, 0);

            modelToRotate.transform.rotation = Quaternion.Slerp(modelToRotate.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            if (barrel != null)
            {
                Quaternion barrelRotation = Quaternion.Euler(0, 0, BarrelZRotationIncrement);
                Quaternion targetBarrelRotation = barrel.transform.rotation * barrelRotation;

                // Apply lerp to achieve smoother start and stop
                barrel.transform.rotation = Quaternion.Lerp(barrel.transform.rotation, targetBarrelRotation, Time.deltaTime * barrelRotationSpeed);

                // Accelerate rotation
                barrelRotationSpeed += barrelRotationAcceleration * Time.deltaTime;
            }
        }
        else
        {
            // Decelerate rotation when not shooting
            barrelRotationSpeed = Mathf.Lerp(barrelRotationSpeed, 0f, Time.deltaTime * barrelRotationAcceleration);
        }
    }

    private bool IsTurretCloseToTargetRotation()
    {
        return Quaternion.Angle(modelToRotate.transform.rotation, Quaternion.LookRotation(modelToRotate.transform.position - target.position, Vector3.up)) < TurretTargetRotationThreshold;
    }

    private void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Zombie");

        if (enemies.Length > 0)
        {
            float minCombinedScore = Mathf.Infinity;
            GameObject bestEnemy = null;

            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                float rotationScore = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(enemy.transform.position - transform.position, Vector3.up));

                float combinedScore = distance + 0.1f * rotationScore;

                if (combinedScore < minCombinedScore && distance <= shootingRange)
                {
                    minCombinedScore = combinedScore;
                    bestEnemy = enemy;
                }
            }

            target = bestEnemy?.transform;
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

        Bullet bulletController = bullet.GetComponent<Bullet>();
        if (bulletController != null)
        {
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            if (bulletRigidbody != null)
            {
                bulletRigidbody.AddForce(-bulletSpawnPoint.forward * bulletForce, ForceMode.Impulse);
            }
        }

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        lastShotTime = Time.time;
    }

    private void DestroyTurret()
    {
        // Perform any cleanup or destruction logic here
        Destroy(gameObject);
    }
}
