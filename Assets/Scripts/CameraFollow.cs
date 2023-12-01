using UnityEngine;

public class PixelPerfectCameraFollow : MonoBehaviour
{
    public Transform target; // The target (player) that the camera will follow
    public float smoothSpeed = 0.125f; // The speed at which the camera will follow the target
    public Vector3 offset; // The offset from the target's position
    public int pixelsPerUnit = 100; // The number of screen pixels that correspond to one unit in the game world

    void FixedUpdate()
    {
        // Calculate the desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Align the camera's position with the pixel grid
        smoothedPosition.x = Mathf.Round(smoothedPosition.x * pixelsPerUnit) / pixelsPerUnit;
        smoothedPosition.y = Mathf.Round(smoothedPosition.y * pixelsPerUnit) / pixelsPerUnit;

        // Update the camera's position
        transform.position = smoothedPosition;
    }
}
