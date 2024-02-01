using System.Diagnostics;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Adjust this to set the movement speed
    public GameObject projectilePrefab; // Assign your projectile prefab in the Unity Editor
    public float projectileOffset = 1.0f; // Adjust this to set the offset from the player's position
    public GameObject shootingDirectionReference; // Assign the GameObject whose forward direction you want to use
    public float sprintSpeedMultiplier = 2f; // Adjust this to set the sprint speed multiplier
    public float sprintDuration = 0.5f; // Adjust this to set the sprint duration in seconds
    public float sprintCooldown = 2f; // Adjust this to set the sprint cooldown in 
    public float shootingCooldown = 0.5f; // Adjust this to set the shooting cooldown in seconds
    public float shootingForce = 100f; // Adjust this to set the force used for shooting the bullet in 

    private float originalSpeed; // Store the original speed before sprinting
    private bool isSprinting = false; // Flag to track if the player is currently sprinting
    private float lastSprintTime = 0f; // Record the time of the last sprint

    private Rigidbody rb;

    public bool canShoot = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        canShoot = true;

        // Assign the initial speed to originalSpeed
        originalSpeed = speed;
    }

    void Update()
    {
        // Get input from arrow keys
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Check for sprint input (only trigger once when Shift key is pressed and after cooldown)
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            if (Time.time > lastSprintTime + sprintCooldown)
            {
                StartCoroutine(StartSprint());
            }
        }

        // Calculate movement direction based on arrow keys
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // Rotate the player towards the mouse position
        RotateTowardsMouse();

        // Apply force to the Rigidbody with sprint multiplier if currently sprinting
        rb.AddForce(movement * (isSprinting ? speed * sprintSpeedMultiplier : speed));

        // Check for shooting input
        if (Input.GetMouseButton(0) && canShoot)
        {
            StartCoroutine(ShootWithCooldown());
        }
    }

    void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            Vector3 direction = targetPosition - transform.position;

            if (direction != Vector3.zero)
            {
                // Apply a 90-degree offset to the rotation
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0, -90, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * 500f);
            }
        }
    }

    void ShootProjectile()
    {
        // Calculate the spawn position in front of the player
        Vector3 spawnPosition = transform.position + transform.right * projectileOffset;

        // Get the forward direction of the shooting direction reference object
        Vector3 shootingDirection = shootingDirectionReference.transform.right;

        // Instantiate a projectile at the calculated position and rotation
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        // Set the y-component of the shooting direction to zero to restrict vertical movement
        shootingDirection.y = 0f;

        // Normalize the shooting direction to maintain the same speed in all directions
        shootingDirection.Normalize();

        // Directly set the initial rotation of the projectile
        projectile.transform.rotation = Quaternion.LookRotation(transform.right);

        // Add force to the projectile in the direction of the specified shooting direction
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null)
        {
            projectileRb.AddForce(shootingDirection * shootingForce, ForceMode.Impulse); // Adjust the force as needed
        }
    }

    IEnumerator ShootWithCooldown()
    {
        if (canShoot)
        {
            ShootProjectile();
            canShoot = false;

            // Wait for the specified shooting cooldown
            yield return new WaitForSeconds(shootingCooldown);

            canShoot = true;
        }
    }
    IEnumerator StartSprint()
    {
        // Temporarily increase the speed for the specified sprint duration
        isSprinting = true;
        speed *= sprintSpeedMultiplier;

        // Wait for the specified sprint duration
        yield return new WaitForSeconds(sprintDuration);

        // Record the time of the last sprint
        lastSprintTime = Time.time;

        // Restore the original speed
        speed = originalSpeed;
        isSprinting = false;
    }


    public float GetRemainingDashCooldown()
    {

        float elapsedTime = Time.time - lastSprintTime;
        return Mathf.Max(0f, sprintCooldown - elapsedTime);
    }
}
