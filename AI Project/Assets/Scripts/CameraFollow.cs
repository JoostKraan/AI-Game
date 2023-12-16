using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 3f, -5f); // Adjust the values to your liking

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player transform not assigned!");
            return;
        }

        // Calculate the desired position for the camera with the offset
        Vector3 targetPosition = player.position + offset;

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
