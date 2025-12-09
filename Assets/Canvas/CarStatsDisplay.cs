using UnityEngine;
using TMPro;

public class CarStatsDisplay : MonoBehaviour
{
    // References to the Text UI elements
    public TMP_Text avgSpeedText;    // Text for average speed
    public TMP_Text maxSpeedText;    // Text for maximum speed
    public TMP_Text distanceText;     // Text for distance traveled
    public TMP_Text lapTimeText;      // Text for lap time (to be implemented)
    public TMP_Text fastestTimeText;
    public TMP_Text idleTimeText;     // Text for idle time
    public TMP_Text lapsCompletedText; // Text for laps completed (added this line)

    // Reference to the CarStatistics script
    private CarStatistics carStatistics;

    void Start()
    {
        // Find the CarStatistics component attached to the car GameObject
        carStatistics = GetComponent<CarStatistics>();
    }

    void Update()
    {
        // Update the text fields with car stats
        if (carStatistics != null)
        {
            avgSpeedText.text = "Average Speed: " + carStatistics.GetAverageSpeed().ToString("F2") + " cm/s";
            maxSpeedText.text = "Max Speed: " + carStatistics.GetMaxSpeed().ToString("F2") + " cm/s";
            distanceText.text = "Distance: " + carStatistics.GetDistanceTraveled().ToString("F2") + " cm";
            lapsCompletedText.text = "Laps Completed: " + carStatistics.GetLapsCompleted(); // Updated to match the declared variable
            lapTimeText.text = "Current Lap Time: " + carStatistics.GetCurrentLapTime().ToString("F2") + " s";
            fastestTimeText.text = "Fastest Lap Time: " + carStatistics.GetMinLapTime();
            idleTimeText.text = "Idle Time: " + carStatistics.GetIdleTime().ToString("F2") + " s";
        }
    }
}
