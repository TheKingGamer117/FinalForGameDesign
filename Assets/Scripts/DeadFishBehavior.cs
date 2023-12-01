using UnityEngine;

public class DeadFishBehavior : MonoBehaviour
{
    [Header("Floating Speed")]
    public float floatSpeed = 2.0f;  // The speed at which the dead fish floats upwards

    [Header("Rotation Speed")]
    public float rotationSpeed = 60f; // The speed of rotation

    private Quaternion targetRotation; // The target rotation

    void Start()
    {
        // Set the target rotation to be 180 degrees around Z-axis
        targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 180);
    }

    void Update()
    {
        // Make the dead fish float towards the water surface at y = 0
        Vector3 moveVector = Vector3.up * floatSpeed * Time.deltaTime;
        transform.localPosition += moveVector;

        // Stop moving upward when reaching the water surface at y = 0
        if (transform.localPosition.y >= 0)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
            floatSpeed = 0;
        }

        // Smoothly rotate the fish so its belly faces upwards
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
