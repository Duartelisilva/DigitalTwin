using UnityEngine;
using TMPro;

public class GlobalStatsManager : MonoBehaviour
{
    public CarStatistics car1Stats;
    public CarStatistics car2Stats;
    public CarStatistics car3Stats;

    public TMP_Text fastestLapText;
    public TMP_Text highestSpeedText;
    public TMP_Text mostLapsCompletedText;

    void Update()
    {
        UpdateGlobalStats();
    }

    private void UpdateGlobalStats()
    {
        // Determine the fastest lap and corresponding car color
        float fastestLapTime = Mathf.Infinity;
        string fastestCarColor = "None";

        // Check car 1
        if (car1Stats.GetMinLapTime() < fastestLapTime)
        {
            fastestLapTime = car1Stats.GetMinLapTime();
            fastestCarColor = "Purple"; // Set specific color name
        }

        // Check car 2
        if (car2Stats.GetMinLapTime() < fastestLapTime)
        {
            fastestLapTime = car2Stats.GetMinLapTime();
            fastestCarColor = "Yellow"; // Set specific color name
        }

        // Check car 3
        if (car3Stats.GetMinLapTime() < fastestLapTime)
        {
            fastestLapTime = car3Stats.GetMinLapTime();
            fastestCarColor = "Blue"; // Set specific color name
        }

        // Update the UI text for fastest lap
        if (fastestLapTime < Mathf.Infinity)
        {
            fastestLapText.text = $"Fastest Time: {fastestLapTime:F2} {fastestCarColor}";
        }
        else
        {
            fastestLapText.text = "Fastest Time: N/A";
        }

        // Update the highest speed (assuming GetMaxSpeed() returns the max speed of the car)
        float highestSpeed = Mathf.Max(car1Stats.GetMaxSpeed(), car2Stats.GetMaxSpeed(), car3Stats.GetMaxSpeed());
        highestSpeedText.text = $"Highest Speed: {highestSpeed:F2} m/s";

        // Determine which car completed the most laps
        int mostLaps = Mathf.Max(car1Stats.GetLapsCompleted(), car2Stats.GetLapsCompleted(), car3Stats.GetLapsCompleted());
        string mostLapsCar = "None";
        
        if (mostLaps == car1Stats.GetLapsCompleted()) mostLapsCar = "Car 1";
        else if (mostLaps == car2Stats.GetLapsCompleted()) mostLapsCar = "Car 2";
        else if (mostLaps == car3Stats.GetLapsCompleted()) mostLapsCar = "Car 3";

        mostLapsCompletedText.text = $"Most Laps Completed: {mostLaps} by {mostLapsCar}";
    }
}
