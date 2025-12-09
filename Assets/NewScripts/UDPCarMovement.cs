using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPCarMovement : MonoBehaviour
{
    public float v = 0f; // Forward velocity of the car
    public float b = 6f; // Distance between front and rear axles
    public float a = 3f; // Distance between front axle and car's center of mass
    public float maxdegrees = 13.436f; // steering limit
    public float decelerationRate = 7f; // strenght of car deceleration

    private float delta; // Steering angle
    private float steeringAngle = 0f; // Current steering angle
    private readonly float[] steeringValues = { -13.436f, -8.9573f, -4.4787f, 0f, 4.4787f, 8.9573f, 13.436f }; // Combined steering angles in degrees
    private int currentSteeringIndex = 3; // starts on default steering, 0º
    float maxSpeed = 91.068f;
    public float centreOfGravityOffset = -0.7f;
    public float acceleration = 273.20f; 
    private Rigidbody rb; // Reference to the Rigidbody component
    private float direction = 1f;

    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    private string receivedCommand;

    private float commandDuration = 0f; // Duration to hold the command
    private float commandDurationTimer = 0f; // Timer for the command duration
    private float commandDelay = 0.1f;
    private bool commandActive = false; // Indicates if a command is currently active

    private GameObject frontaxis, frontleft, frontright;
    private Quaternion initialFrontAxisRotation, initialFrontLeftWheelRotation, initialFrontRightWheelRotation;



    public int port;
    void Start()
    {
        // Get the reference to the Rigidbody component
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += Vector3.up * centreOfGravityOffset;

        // Setup UDP listener
        udpClient = new UdpClient(port); // Listen on port 12345
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        Debug.Log("UDP Listener started on port " + port);

        frontaxis = transform.Find("front axis").gameObject;
        frontleft = transform.Find("front left").gameObject;
        frontright = transform.Find("front right").gameObject;
            if (frontaxis == null)
            {
                Debug.LogError("Child object 'rodas frente' not found!");
            }
            else
            {
                // Set the default rotation to 90 degrees to the left
                initialFrontAxisRotation = frontaxis.transform.localRotation;
                initialFrontLeftWheelRotation = frontleft.transform.localRotation;
                initialFrontRightWheelRotation = frontright.transform.localRotation;
            }

        // Start the UDP listening coroutine
        StartCoroutine(ListenForCommands());
    }

    void FixedUpdate()
    {
        // Check if there is a movement command active the car cannot receive new commands
        if (commandActive)
        {
            commandDurationTimer -= Time.fixedDeltaTime;
            if (commandDurationTimer <= 0f)
            {
                v = 0f; // Reset velocity after the command duration
                commandActive = false; // Deactivate flag
            }
        }

        // Calculate the steering angle based on the current steering index
        delta = (steeringValues[currentSteeringIndex]) * Mathf.Deg2Rad;
        delta = Mathf.Clamp(delta, -maxdegrees * Mathf.Deg2Rad, maxdegrees * Mathf.Deg2Rad);


        if(v > 0)
        direction = 1;
        else if(v < 0)
        direction = -1;

        // Velocity calculations
        float alfa = Mathf.Atan(a * Mathf.Tan(delta) / b); // Steering angle of the front wheels
        float theta = transform.eulerAngles.y * Mathf.Deg2Rad; // Convert car's rotation to radians
        float x_dot = v * Mathf.Cos(alfa + theta); // x velocity
        float y_dot = v * Mathf.Sin(alfa + theta); // y velocity
        float theta_dot = rb.velocity.magnitude * direction / b * Mathf.Tan(delta); // Angular velocity

        // Calculate the movement direction based on the forward direction
        Vector3 movementDirection = Vector3.ClampMagnitude(new Vector3(-y_dot, 0f, -x_dot), 1f);
        movementDirection = Vector3.ProjectOnPlane(movementDirection, Vector3.up).normalized;

        // Calculate the force to apply the movement
        Vector3 force = Vector3.Dot(movementDirection, transform.forward) * transform.forward * acceleration * Time.deltaTime;

        // Apply the force to the Rigidbody
        rb.AddForce(force, ForceMode.VelocityChange);

        // Friction when there is no movement command active
        if (v == 0)
        {
            Vector3 velocity = rb.velocity;
            Vector3 deceleration = -velocity * decelerationRate;
            rb.AddForce(deceleration, ForceMode.Acceleration);
        }

        // Apply damping to reduce sideways drifting
        float slippingFactor = 0.3f;
        Vector3 relativeVelocity = transform.InverseTransformDirection(rb.velocity);
        relativeVelocity.x *= slippingFactor;
        rb.velocity = transform.TransformDirection(relativeVelocity);

        // Limit the speed
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed - 5);

        // rotate the car
        Quaternion deltaRotation = Quaternion.Euler(0f, theta_dot * Mathf.Rad2Deg * Time.deltaTime, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);

        // Calculate the steering angle for the front wheels rotation
        steeringAngle = steeringValues[currentSteeringIndex];
        delta = (steeringAngle) * Mathf.Deg2Rad;
        delta = Mathf.Clamp(delta, -maxdegrees * Mathf.Deg2Rad, maxdegrees * Mathf.Deg2Rad);
        float targetRotationAngle = Mathf.Clamp(steeringValues[currentSteeringIndex], -maxdegrees, maxdegrees);

        // Apply the steering rotation to each front wheel and the front axis
        if (frontleft != null && frontright != null && frontaxis != null)
        {
            frontaxis.transform.localRotation = initialFrontAxisRotation * Quaternion.Euler(targetRotationAngle, 0f, 0f);
            frontleft.transform.localRotation = initialFrontLeftWheelRotation * Quaternion.Euler(targetRotationAngle, 0f, 0f);
            frontright.transform.localRotation = initialFrontRightWheelRotation * Quaternion.Euler(targetRotationAngle, 0f, 0f);
        }

        Vector3 extraGravity = new Vector3(0, -0f, 0); // You can adjust the -20 to a stronger downward force.
        rb.AddForce(extraGravity, ForceMode.Acceleration);
    }

    private IEnumerator ListenForCommands()
    {
        while (true)
        {
            if (udpClient.Available > 0)
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                receivedCommand = Encoding.ASCII.GetString(data);
                ExecuteCommand(receivedCommand);
            }
            yield return null; // Wait for the next frame
        }
    }

    private void ExecuteCommand(string command)
    {
        if(!commandActive){
            // Execute the command and refresh it immediately
            if (command == "mf")
            {
                SetVelocity(1f, commandDelay); // Move forward for 100 ms
            }
            else if (command == "mb")
            {
                SetVelocity(-1f, commandDelay); // Move backward for 100 ms
            }
            else if (command == "ml")
            {
                // Change steering to left
                currentSteeringIndex = Mathf.Min(currentSteeringIndex, 3);
                currentSteeringIndex = Mathf.Clamp(currentSteeringIndex - 1, 0, steeringValues.Length - 1);
                steeringAngle = steeringValues[currentSteeringIndex]; // Update steering angle
                SetVelocity(0f, commandDelay);
            }
            else if (command == "mr")
            {
                // Change steering to right
                currentSteeringIndex = Mathf.Max(currentSteeringIndex, 3);
                currentSteeringIndex = Mathf.Clamp(currentSteeringIndex + 1, 0, steeringValues.Length - 1);
                steeringAngle = steeringValues[currentSteeringIndex]; // Update steering angle
                SetVelocity(0f, commandDelay);
            }
            else if (command == "rr")
            {
                // Reset steering to default position
                currentSteeringIndex = 3;
                steeringAngle = 0f; // Reset steering angle to center
                SetVelocity(0f, commandDelay);
            }
            else if (command == "+d")
            {
                // Increase command delay by 100ms
                commandDelay += 0.1f;
                Debug.Log($"Command delay increased to: {commandDelay} seconds");
                SetVelocity(0f, commandDelay);
            }
            else if (command == "-d")
            {
                // Decrease command delay by 100ms, ensuring it doesn't go below 0
                commandDelay = Mathf.Max(0.1f, commandDelay - 0.1f);
                Debug.Log($"Command delay decreased to: {commandDelay} seconds");
                SetVelocity(0f, commandDelay);
            }
        }
    }

    private void SetVelocity(float targetVelocity, float duration)
    {
        v = targetVelocity; // Set the desired velocity
        commandDuration = duration; // Set the duration of the command
        commandDurationTimer = duration; // Reset the command timer
        commandActive = true; // Mark command as active
    }

    void OnApplicationQuit()
    {
        if (udpClient != null)
        {
            udpClient.Close();
            Debug.Log("UDP Listener closed.");
        }
    }
}