using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceToCamera : MonoBehaviour
{
    void Update()
    {
        // Face the camera
        transform.LookAt(Camera.main.transform);

        // Optionally, you can also rotate the text by 180 degrees
        transform.Rotate(0, 180, 0);
    }
}
