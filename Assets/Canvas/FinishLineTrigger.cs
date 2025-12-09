using UnityEngine;

public class FinishLineTrigger : MonoBehaviour
{
    // Reference to the CarStatistics component
    private CarStatistics carStatistics;

    void Start()
    {
        // Find the CarStatistics component attached to the car GameObject
        carStatistics = FindObjectOfType<CarStatistics>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has the CarStatistics script
        if (other.CompareTag("Car")) // Make sure to set the tag for your car GameObject
        {
            // Call the method to handle lap completion
            if (carStatistics != null)
            {
                carStatistics.CompleteLap(); // Call the complete lap method
            }
        }
    }
}
