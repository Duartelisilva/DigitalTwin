using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class CarController : MonoBehaviour
{
    // Network parameters
    private UdpClient udpClient;
    private IPEndPoint carEndPoint;
    public string carIP = "192.168.137.255"; // IP of the car
    public int carPort = 8080;

    // Movement variables
    public float movementTime = 1.0f; // Movement duration in seconds
    private float movementIncrement = 0.5f; // Change movement time by this value

    void Start()
    {
        // Initialize UDP client and endpoint
        udpClient = new UdpClient();
        carEndPoint = new IPEndPoint(IPAddress.Parse(carIP), carPort);
        Debug.Log("Car Controller initialized.");
    }

    void Update()
    {
        // Movement commands
        if (Input.GetKeyDown(KeyCode.UpArrow)) // Forward (mf)
        {
            SendMessageToCar("mf");
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) // Backward (mb)
        {
            SendMessageToCar("mb");
        }

        // Increase/decrease movement time
        if (Input.GetKeyDown(KeyCode.W)) // Increase time (+d)
        {
            movementTime += movementIncrement;
            //Debug.Log("Movement time increased: " + movementTime + "s");
            SendMessageToCar("+d");
        }
        if (Input.GetKeyDown(KeyCode.D)) // Decrease time (-d)
        {
            movementTime = Mathf.Max(0.5f, movementTime - movementIncrement);
            //Debug.Log("Movement time decreased: " + movementTime + "s");
            SendMessageToCar("-d");
        }

        // Steering commands
        if (Input.GetKeyDown(KeyCode.LeftArrow)) // Left (ml)
        {
            SendMessageToCar("ml");
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) // Right (mr)
        {
            SendMessageToCar("mr");
        }
        if (Input.GetKeyDown(KeyCode.R)) // Reset steering (rr)
        {
            SendMessageToCar("rr");
        }

        // Flash LED command to check if the car is alive (flash)
        if (Input.GetKeyDown(KeyCode.F)) // Flash onboard LED
        {
            SendMessageToCar("flash");
        }
    }

    // Function to send a message to the car
    void SendMessageToCar(string message)
    {
        try
        {
            // Convert the message to bytes and send via UDP
            byte[] data = Encoding.ASCII.GetBytes(message);
            udpClient.Send(data, data.Length, carEndPoint);
           // Debug.Log("Command sent: " + message);
        }
        catch (System.Exception e)
        {
           // Debug.LogError("Error sending message: " + e.Message);
        }
    }

    // Cleanup UDP connection when the game object is destroyed
    private void OnDestroy()
    {
        if (udpClient != null)
        {
            udpClient.Close(); // Close the UDP connection if it's instantiated
          //  Debug.Log("UDP Client closed.");
        }
        else
        {
           // Debug.LogWarning("udpClient was null on destruction.");
        }
    }
}
