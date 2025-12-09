using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Globalization;

public class VehicleManager : MonoBehaviour
{
    // References to the yellow, blue, and purple cars
    public GameObject yellow_car;
    public GameObject blue_car;
    public GameObject purple_car;

    public GameObject ghost_yellow_car;
    public GameObject ghost_blue_car;
    public GameObject ghost_purple_car;

    // Variables to store the position and direction of each car
    public Vector3 yellowCarPosition;
    public Vector3 blueCarPosition;
    public Vector3 purpleCarPosition;
    public float yellowdirection;
    public float bluedirection;
    public float purpledirection;

    // Variables to store the last safe positions and directions of the cars
    private Vector3 yellowCarSafePosition;
    private Vector3 blueCarSafePosition;
    private Vector3 purpleCarSafePosition;
    private Vector3 yellowlastsafeposition;
    private Vector3 bluelastsafeposition;
    private Vector3 purplelastsafeposition;
    private float yellowCarSafeDirection;
    private float blueCarSafeDirection;
    private float purpleCarSafeDirection;

    private UdpClient udpClient;
    private Thread udpThread;
    private bool isRunning = true;
    public int port = 8085;

    private int auxyellow, auxpurple, auxblue = 0;

    // race track limits
    private float minX = -200f;
    private float maxX = -5f;
    private float minZ = -235f;
    private float maxZ = -5f;

    // filter to eliminate noise on sideways car movement
    public float xAxisMovementThreshold = 15f;

    private bool reset_position_blue, reset_position_yellow, reset_position_purple;

    // flags to indicate new data received
    private volatile bool yellowDataReceived = false;
    private volatile bool blueDataReceived = false;
    private volatile bool purpleDataReceived = false;
    
    public CarDataDatabase carDataDatabase;

    void Start()
    {
        // start UDP listener
        udpClient = new UdpClient(port);
        udpThread = new Thread(new ThreadStart(ReceiveData));
        udpThread.IsBackground = true;
        udpThread.Start();

        // Set initial positions
        SetInitialPositions();

        carDataDatabase = FindObjectOfType<CarDataDatabase>();
    }

    // close UDP listener
    void OnDestroy()
    {
        isRunning = false;
        udpClient.Close();
        udpThread.Join();
    }

    void FixedUpdate()
    {
        // Check if new data is available and update car positions if safe
        UpdateCarPositions();

        // Check if cars are in a safe position
        CheckCarSafety();
        SaveCarDataToDatabase();

        // bypass the unsafe trigger and place the Virtual Cars on the Ghost Car's position, if the new position is within the race track's limits
        if (Input.GetKey(KeyCode.O))
        {
            reset_position_blue = true;
            reset_position_yellow = true;
            reset_position_purple = true;
        }
    }

    private void SetInitialPositions()
    {
        yellowCarPosition = new Vector3(-30f, 0.1f, -180f);
        blueCarPosition = new Vector3(-15f, 0.1f, -180f);
        purpleCarPosition = new Vector3(-10f, 0.1f, -180f);

        yellow_car.transform.position = yellowCarPosition;
        blue_car.transform.position = blueCarPosition;
        purple_car.transform.position = purpleCarPosition;

        // Initialize safe positions and directions
        yellowCarSafePosition = yellowCarPosition;
        blueCarSafePosition = blueCarPosition;
        purpleCarSafePosition = purpleCarPosition;
        yellowCarSafeDirection = 0f; 
        blueCarSafeDirection = 0f; 
        purpleCarSafeDirection = 0f;

        // force the Virtual Cars to overlap the Ghost Cars when the code is initiated
        reset_position_blue = true;
        reset_position_yellow = true;
        reset_position_purple = true;
    }

    private void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        while (isRunning)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                ParseMessage(message);
            }
            catch (System.Exception ex)
            {
                Debug.Log("Error receiving data: " + ex.Message);
            }
        }
    }

    // extracts data received from the string and updates the car position and direction's values
    private void ParseMessage(string message)
    {
        string[] parts = message.Split(',');

        // Validate the message format
        if (parts.Length == 5)
        {
            string carId = parts[0];
            if (float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float z) &&
                float.TryParse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture, out float direction))
            {
                switch (carId)
                {
                    case "yellow":
                        yellowCarPosition = new Vector3(x, y, z);
                        yellowdirection = direction;
                        yellowDataReceived = true; // Set flag for yellow car data
                        break;
                    case "blue":
                        blueCarPosition = new Vector3(x, y, z);
                        bluedirection = direction;
                        blueDataReceived = true; // Set flag for blue car data
                        break;
                    case "purple":
                        purpleCarPosition = new Vector3(x, y, z);
                        purpledirection = direction;
                        purpleDataReceived = true; // Set flag for purple car data
                        break;
                    default:
                        Debug.Log("Unknown car ID: " + carId);
                        break;
                }
            }
            else
            {
                Debug.LogError("Failed to parse position or direction as float.");
            }
        }
        else
        {
            Debug.LogError("Invalid message format: " + message);
        }
    }

    // updates the car's positions
    private void UpdateCarPositions()
    {
        if (yellow_car != null && yellowDataReceived && IsCarInSafePosition(yellow_car, yellowCarPosition, yellowdirection, yellowCarSafePosition, reset_position_yellow))
        {
            yellow_car.transform.position = yellowCarPosition;
            yellow_car.transform.rotation = Quaternion.Euler(0f, yellowdirection, 0f);

            // verifies if the received values are within the race track's outer limits
            if((yellowCarPosition.x > minX) && (yellowCarPosition.x < maxX) && (yellowCarPosition.z > minZ) && (yellowCarPosition.z < maxZ))
            {
                reset_position_yellow = false;
                yellowlastsafeposition = yellowCarSafePosition;
                yellowCarSafePosition = yellowCarPosition; // Update safe position if successful
                yellowCarSafeDirection = yellowdirection; // Update safe direction if successful
            }
        }
        else
        {
            yellowDataReceived = false; // Reset flag if position update was unsuccessful
        }

        if (blue_car != null && blueDataReceived && IsCarInSafePosition(blue_car, blueCarPosition, bluedirection, blueCarSafePosition, reset_position_blue))
        {
            blue_car.transform.position = blueCarPosition;
            blue_car.transform.rotation = Quaternion.Euler(0f, bluedirection, 0f);
            if(blueCarPosition.x > minX && blueCarPosition.x < maxX && blueCarPosition.z > minZ && blueCarPosition.z < maxZ)
            {
                reset_position_blue = false;
                bluelastsafeposition = blueCarSafePosition;
                blueCarSafePosition = blueCarPosition; 
                blueCarSafeDirection = bluedirection; 
            }
        }
        else
        {
            blueDataReceived = false;
        }

        if (purple_car != null && purpleDataReceived && IsCarInSafePosition(purple_car, purpleCarPosition, purpledirection, purpleCarSafePosition, reset_position_purple))
        {
            purple_car.transform.position = purpleCarPosition;
            purple_car.transform.rotation = Quaternion.Euler(0f, purpledirection, 0f);
            if(purpleCarPosition.x > minX && purpleCarPosition.x < maxX && purpleCarPosition.z > minZ && purpleCarPosition.z < maxZ)
            {
                reset_position_purple = false;
                purplelastsafeposition = purpleCarSafePosition;
                purpleCarSafePosition = purpleCarPosition; 
                purpleCarSafeDirection = purpledirection; 
            }
        }
        else
        {
            purpleDataReceived = false;
        }
    }

    private void CheckCarSafety()
    {
        if (!IsCarInSafePosition(yellow_car, yellowCarPosition, yellowdirection, yellowCarSafePosition, false))
        {
            Debug.Log("Yellow car is not in a safe position. Reverting to last safe position.");
            RestoreCarToSafePosition(yellow_car, yellowCarSafePosition, yellowCarSafeDirection);
        }

        if (!IsCarInSafePosition(blue_car, blueCarPosition, bluedirection, blueCarSafePosition, false))
        {
            Debug.Log("Blue car is not in a safe position. Reverting to last safe position.");
            RestoreCarToSafePosition(blue_car, blueCarSafePosition, blueCarSafeDirection);
        }

        if (!IsCarInSafePosition(purple_car, purpleCarPosition, purpledirection, purpleCarSafePosition, false))
        {
            Debug.Log("Purple car is not in a safe position. Reverting to last safe position.");
            RestoreCarToSafePosition(purple_car, purpleCarSafePosition, purpleCarSafeDirection);
        }
    }

    private void RestoreCarToSafePosition(GameObject car, Vector3 safePosition, float safeDirection)
    {
        if (car != null)
        {
            car.transform.position = safePosition; // Restore position
            car.transform.rotation = Quaternion.Euler(0f, safeDirection, 0f); // Restore direction
        }
    }

    private bool IsCarInSafePosition(GameObject car, Vector3 currentPosition, float currentDirection, Vector3 lastSafePosition, bool reset)
    {

        return (IsCarOnTrack(car) && (!HasCarBeenGrabbed(car, currentPosition, lastSafePosition) || reset));
    }

    // Check if the car is on the track surface by emitting a ray downwards to detect the surface
    private bool IsCarOnTrack(GameObject car)
    {
        Vector3 rayOrigin = car.transform.position + Vector3.up * 0.5f; // 0.5 units above the car
        Ray ray = new Ray(rayOrigin, Vector3.down);
        RaycastHit hit;
        int trackLayerMask = LayerMask.GetMask("TrackSurface");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, trackLayerMask))
        {
            if (hit.collider.CompareTag("Track"))
            {
                return true;
            }
        }
        Debug.Log($"{car.name} is off the track.");
        return false;
    }

    // verifies if the car had an abrupt sideways movement 
    private bool HasCarBeenGrabbed(GameObject car, Vector3 currentPosition, Vector3 lastSafePosition)
    {
        // Convert the current and last safe positions to the car's local space
        Vector3 localCurrentPosition = car.transform.InverseTransformPoint(currentPosition);
        Vector3 localLastSafePosition = car.transform.InverseTransformPoint(lastSafePosition);

        // Check movement along the local X-axis
        float xAxisMovement = Mathf.Abs(localCurrentPosition.x - localLastSafePosition.x);
        return xAxisMovement > xAxisMovementThreshold;
    }

    // Method to get the last safe positions of the cars
    public Vector3 GetYellowCarSafePosition() { return yellowCarSafePosition; }
    public Vector3 GetBlueCarSafePosition() { return blueCarSafePosition; }
    public Vector3 GetPurpleCarSafePosition() { return purpleCarSafePosition; }

    // Method to get the last safe directions of the cars
    public float GetYellowCarSafeDirection() { return yellowCarSafeDirection; }
    public float GetBlueCarSafeDirection() { return blueCarSafeDirection; }
    public float GetPurpleCarSafeDirection() { return purpleCarSafeDirection; }

    // saves the timestamp, coordinates, speed, and direction of the cars to the database 
    private void SaveCarDataToDatabase()
    {
        float deltaTime = Time.fixedDeltaTime; // Time between FixedUpdate calls
        float fluctuationThreshold = 0.05f; // Threshold to filter fluctuations on the values

        // Yellow Car
        Vector3 yellowPositionDelta = yellowCarSafePosition - yellowlastsafeposition;
        Vector3 yellowLocalspeed = yellow_car.transform.InverseTransformDirection(yellowPositionDelta / deltaTime);

        // apply fluctuation threshold and max speed limit
        yellowLocalspeed.x = Mathf.Abs(yellowLocalspeed.x) < fluctuationThreshold ? 0 : yellowLocalspeed.x;
        yellowLocalspeed.z = Mathf.Abs(yellowLocalspeed.z) < fluctuationThreshold ? 0 : yellowLocalspeed.z;
        if (Mathf.Abs(yellowLocalspeed.x) > 200 || Mathf.Abs(yellowLocalspeed.z) > 200)
        {
            // Set speed to (0, 0, 0) if the speed value was highly above the limit to filter errors
            yellowLocalspeed = Vector3.zero;
        }
        Vector3 yellowCarspeed = new Vector3(yellowLocalspeed.x, 0, -yellowLocalspeed.z);
        carDataDatabase.SaveCarData("Yellow Car", -yellowCarSafePosition, yellowCarspeed, yellowdirection);

        Vector3 bluePositionDelta = blueCarSafePosition - bluelastsafeposition;
        Vector3 blueLocalspeed = blue_car.transform.InverseTransformDirection(bluePositionDelta / deltaTime);

        // Blue Car
        blueLocalspeed.x = Mathf.Abs(blueLocalspeed.x) < fluctuationThreshold ? 0 : blueLocalspeed.x;
        blueLocalspeed.z = Mathf.Abs(blueLocalspeed.z) < fluctuationThreshold ? 0 : blueLocalspeed.z;
        if (Mathf.Abs(blueLocalspeed.x) > 200 || Mathf.Abs(blueLocalspeed.z) > 200)
        {
            blueLocalspeed = Vector3.zero; 
        }
        Vector3 blueCarspeed = new Vector3(blueLocalspeed.x, 0, -blueLocalspeed.z);
        carDataDatabase.SaveCarData("Blue Car", -blueCarSafePosition, blueCarspeed, bluedirection);

        // Purple Car
        Vector3 purplePositionDelta = purpleCarSafePosition - purplelastsafeposition;
        Vector3 purpleLocalspeed = purple_car.transform.InverseTransformDirection(purplePositionDelta / deltaTime);

        purpleLocalspeed.x = Mathf.Abs(purpleLocalspeed.x) < fluctuationThreshold ? 0 : purpleLocalspeed.x;
        purpleLocalspeed.z = Mathf.Abs(purpleLocalspeed.z) < fluctuationThreshold ? 0 : purpleLocalspeed.z;
        if (Mathf.Abs(purpleLocalspeed.x) > 200 || Mathf.Abs(purpleLocalspeed.z) > 200)
        {
            purpleLocalspeed = Vector3.zero; // Set speed to (0, 0, 0)
        }
        Vector3 purpleCarspeed = new Vector3(purpleLocalspeed.x, 0, -purpleLocalspeed.z);
        carDataDatabase.SaveCarData("Purple Car", -purpleCarSafePosition, purpleCarspeed, purpledirection);
    }

}