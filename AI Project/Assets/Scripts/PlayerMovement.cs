using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Adjust this to set the movement speed
    public GameObject projectilePrefab; // Assign your projectile prefab in the Unity Editor
    public float projectileOffset = 1.0f; // Adjust this to set the offset from the player's position
    public GameObject shootingDirectionReference; // Assign the GameObject whose forward direction you want to use

    private Rigidbody rb;

    public bool canShoot = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Freeze rotation so the player doesn't tip over
        canShoot = true;
    }

    void Update()
    {
        // Get input from arrow keys
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction based on arrow keys
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // Rotate the player towards the mouse position
        RotateTowardsMouse();

        // Apply force to the Rigidbody
        rb.AddForce(movement * speed);

        // Check for shooting input
        if (Input.GetMouseButtonDown(0) && canShoot) // Change to the appropriate mouse button (0 for left, 1 for right, 2 for middle)
        {
            ShootProjectile();
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

        // Instantiate a projectile at the calculated position and rotation
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        // Get the forward direction of the shooting direction reference object
        Vector3 shootingDirection = shootingDirectionReference.transform.right;

        // Set the y-component of the shooting direction to zero to restrict vertical movement
        shootingDirection.y = 0f;

        // Normalize the shooting direction to maintain the same speed in all directions
        shootingDirection.Normalize();

        // Apply rotation to the projectile to match the specified shooting direction
        //float angle = Mathf.Atan2(shootingDirection.y, shootingDirection.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.LookRotation(projectile.GetComponent<Rigidbody>().velocity);

        // Add force to the projectile in the direction of the specified shooting direction
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null)
        {
            projectileRb.AddForce(shootingDirection * 10f, ForceMode.Impulse); // Adjust the force as needed
        }
    }
}
