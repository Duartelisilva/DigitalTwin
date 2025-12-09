using UnityEngine;
using System.Collections;


public class CarStatistics : MonoBehaviour
{
    // Variables for tracking the car's statistics
    private Vector3 lastPosition;
    private float distanceTraveled = 0f;
    private float speed = 0f;
    private float maxSpeed = 0f;
    private float idleTime = 0f; // time spent not moving
    private float averageSpeed = 0f;
    private int speedCount = 0; // Number of speed calculations
    private float updateInterval = 0.1f; // Update stats every 0.1 seconds
    private float timeSinceLastUpdate = 0f;

    // Lap tracking variables
    private bool isOnFinishLine = false;
    private bool isLapActive = false;
    private float lapTime = 0f;
    private int lapsCompleted = 0;
    private bool hasExitedFinishLine = false;
    private float currentTime = 0f;

    private bool aux = false;
    private Transform carTransform;
    private int checkpointindex = 0;
    private bool IsPositionFirst = false;

    private bool canUpdate = false; // Flag to start Update logic
    private float delay = 3f; // Delay in seconds (gives time for the cars' initial positions to be set)
    
    void Start()
    {
        carTransform = this.transform;
        // Initialize the car's last position to the current position at start
        lastPosition = transform.position;
        StartCoroutine(StartUpdateAfterDelay());
    }

    void Update()
    {
        // Track time between updates
        timeSinceLastUpdate += Time.deltaTime;

        // Only update every 'updateInterval' seconds
        if (timeSinceLastUpdate >= updateInterval)
        {
            CalculateStatistics();
            timeSinceLastUpdate = 0f;  // Reset the timer
        }
        // If the lap is active, increment the lap time
        if (isLapActive)
        {
            currentTime += Time.deltaTime;
        }
    }

    void CalculateStatistics()
    {
        // Calculate the distance traveled since the last update
        Vector3 currentPosition = transform.position;
        if(IsPositionFirst && canUpdate){
        float distanceThisFrame = Vector3.Distance(lastPosition, currentPosition);

        // Accumulate the total distance traveled
        distanceTraveled += distanceThisFrame;

        // Calculate current speed (distance/time), in meters per second
        speed = distanceThisFrame / updateInterval;

        // Update the max speed
        if (speed > maxSpeed)
        {
            maxSpeed = speed;
        }

        // Calculate the average speed using a running average formula
        if (speed > 0.1f)  // Only include non-idle speeds
        {
            // Running average formula
            averageSpeed = ((averageSpeed * speedCount) + speed) / (speedCount + 1);
            speedCount++;  // Increment the count of speed updates
        }

        // Check if the car is idle (speed is near 0)
        if (speed < 0.1f) 
        {
            idleTime += updateInterval;
        }

        // Store the current position for the next calculation
        }
        lastPosition = currentPosition;
        IsPositionFirst = true;
    }

    // used for when the car collides with the hitbox of the checkpoints or the finish line
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("FinishLine") && (!isLapActive || checkpointindex==5))
        {
            isOnFinishLine = true; // Update the status

            if(!aux)
                StartLap();

            if (other.CompareTag("FinishLine"))
            {
                // Car has exited the finish line
                isOnFinishLine = false; // Update the status
                hasExitedFinishLine = true; // Mark that the car has exited the finish line

                if (isLapActive)
                {
                    CompleteLap(); // Complete the lap when exiting the finish line
                    checkpointindex = 0;
                }
            }
        }

        // updates whenever the car passes through the next checkpoint
        else if(other.CompareTag("Checkpoint") && checkpointindex==0)
            checkpointindex = 1;

        else if(other.CompareTag("Checkpoint2") && checkpointindex==1)
            checkpointindex = 2;

        else if(other.CompareTag("Checkpoint3") && checkpointindex==2)
            checkpointindex = 3;

        else if(other.CompareTag("Checkpoint4") && checkpointindex==3)
            checkpointindex = 4;

        else if(other.CompareTag("Checkpoint5") && checkpointindex==4)
            checkpointindex = 5;
    }

    private void StartLap()
    {
        isLapActive = true; // Set lap to active
        aux = true;
        if(checkpointindex==5){
        lapsCompleted++; // Increment laps completed
        }
    }

    public void CompleteLap()
    {   
        if(aux){
            if(lapsCompleted > 0){
            
                if ((lapTime == 0 || (currentTime < lapTime)) && checkpointindex==5)
                    lapTime = currentTime;

            }
            Debug.Log($"Lap {lapsCompleted} completed in {lapTime:F2} seconds");
        }
        currentTime = 0;
        aux = false;
    }
    // Public methods to retrieve statistics
    public float GetDistanceTraveled()
    {
        return distanceTraveled;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public float GetIdleTime()
    {
        return idleTime;
    }

    public float GetAverageSpeed()
    {
        return averageSpeed;
    }

    public int GetLapsCompleted()
    {
        return lapsCompleted; // Return the number of completed laps
    }

    public float GetCurrentLapTime()
    {
        return currentTime; // Return the current lap time
    }

    public float GetMinLapTime()
    {
        return lapTime;
    }

    private IEnumerator StartUpdateAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        canUpdate = true; // Enable Update logic after the delay
    }

}
