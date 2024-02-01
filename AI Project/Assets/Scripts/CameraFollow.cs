using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float maxMouseOffsetDistance = 5f;
    public Vector3 additionalOffset = new Vector3(0f, 3f, -5f);

    private float savedX;
    private float savedY;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player transform not assigned!");
            return;
        }

        // Calculate the desired position for the camera with the offsets
        Vector3 targetPosition = player.position;

        savedX += Input.GetAxisRaw("Mouse X");
        savedY += Input.GetAxisRaw("Mouse Y");

        // Calculate the mouse offset
        Vector3 mouseOffset = new Vector3(savedX, 0f, savedY) * mouseSensitivity;
        mouseOffset = Vector3.ClampMagnitude(mouseOffset, maxMouseOffsetDistance);

        // Add the mouse offset and additional offset to the target position
        targetPosition += mouseOffset + additionalOffset;

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
