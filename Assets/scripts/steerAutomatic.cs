using UnityEngine;

public class WheelsSteer : MonoBehaviour
{
    public GameObject carro;
    public Transform rotationAxis2;
    public float maxRotationAngle = 13.436f; // Maximum rotation angle in degrees
    public float rotationSpeed = 100f;

    private Quaternion initialRotation; // Store the initial rotation
    private float currentRotationAngle; // Store the current rotation angle

    void Start()
    {
        // Store the initial rotation
        initialRotation = transform.localRotation;
        currentRotationAngle = 0f; // Initialize current rotation angle
    }

    void FixedUpdate()
    {   
        movimentoautomatico automaticoScript = carro.GetComponent<movimentoautomatico>();
        float rotationInput = automaticoScript.wantedangle;

        // Calculate target rotation angle
        float targetRotationAngle = Mathf.Clamp(rotationInput, -maxRotationAngle, maxRotationAngle);

        // Smoothly interpolate the rotation angle
        currentRotationAngle = Mathf.Lerp(currentRotationAngle, targetRotationAngle, rotationSpeed * Time.deltaTime);

        // Apply the rotation
        transform.localRotation = initialRotation * Quaternion.Euler(currentRotationAngle, 0f, 0f);
    }
}
