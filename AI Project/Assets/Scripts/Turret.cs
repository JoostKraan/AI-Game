using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float shootingCooldown = 1f;
    public float shootingRange = 10f;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletForce = 10f; // Adjust the force as needed

    private Transform target;
    private float lastShotTime;

    private void Start()
    {
        FindNearestEnemy();
    }

    private void Update()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) <= shootingRange)
        {
            // Rotate towards the target on the Y-axis only
            RotateTowardsTarget();

            // Check if the turret is close enough to the target rotation and enough time has passed since the last shot
            if (IsTurretCloseToTargetRotation() && Time.time - lastShotTime >= shootingCooldown)
            {
                // Shoot bullets
                Shoot();
            }
        }
        else
        {
            // If there's no target or target is out of range, find the nearest enemy
            FindNearestEnemy();
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f; // Ignore the Y-axis component

        // Check if the target is in range before updating rotation
        if (Vector3.Distance(transform.position, target.position) <= shootingRange)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);

            // Introduce randomness to the rotation
            float randomAngle = Random.Range(-50f, 50f); // Adjust the range as needed
            toRotation *= Quaternion.Euler(0, randomAngle, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private bool IsTurretCloseToTargetRotation()
    {
        // Check if the turret is close enough to the target rotation (within 1 degree)
        return Quaternion.Angle(transform.rotation, Quaternion.LookRotation(target.position - transform.position, Vector3.up)) < 10.0f;
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

                // Adjust the weighting by multiplying the rotation score by a factor (e.g., 0.1) to give more importance to distance
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
        // Instantiate a bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        // Set the bullet's direction
        Bullet bulletController = bullet.GetComponent<Bullet>();
        if (bulletController != null)
        {
            // Apply force to the bullet
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            if (bulletRigidbody != null)
            {
                bulletRigidbody.AddForce(bulletSpawnPoint.forward * bulletForce, ForceMode.Impulse);
            }
        }

        // Update the last shot time
        lastShotTime = Time.time;
    }
}
