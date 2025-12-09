using UnityEngine;

public class ManualSteer: MonoBehaviour
{
    public float maxSteeringDegrees = 13.436f;
    private Quaternion initialRotation; 

    void Start()
    {
        // Store the initial rotation
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Get steering input
        float steering = Input.GetAxis("Horizontal");

        // Calculate target rotation angle
        float targetRotationAngle = Mathf.Clamp(steering * maxSteeringDegrees, -maxSteeringDegrees, maxSteeringDegrees);

        // Apply the rotation
        transform.localRotation = initialRotation * Quaternion.Euler(targetRotationAngle, 0f, 0f);
    }
}
