using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Globalization;
using System.Collections;

public class GhostCarSender : MonoBehaviour
{
    public string ipAddress = "127.0.0.1"; // IP address of the receiving computer
    public int port = 8085; // Port number of the receiving script
    public float sendDelay = 0f; // Delay in seconds before sending the data

    private UdpClient udpClient;

    // References to ghost cars' child objects (that define the position and direction)
    public Transform yellowCarChild;
    public Transform blueCarChild;
    public Transform purpleCarChild;
    public bool pauseSP = false;
    private bool old_p_pressed = false;

    void Start()
    {
        udpClient = new UdpClient(); // Initialize UDP client
    }

    void OnDestroy()
    {
        udpClient.Close(); // Close the UDP connection when the script is destroyed
    }

    // Coroutine to send car data with a delay
    private IEnumerator SendCarDataWithDelay(string carId, Vector3 position, float direction)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(sendDelay);
        
        // Format the message as carId,x,y,z,direction, using InvariantCulture to ensure proper float formatting
        string message = string.Format(CultureInfo.InvariantCulture, 
            "{0},{1:F4},{2:F4},{3:F4},{4:F4}", 
            carId, 
            position.x, 
            position.y, 
            position.z, 
            direction);
        
        byte[] data = Encoding.UTF8.GetBytes(message); // Convert the message to bytes

        // Send the message to the UDP server
        udpClient.Send(data, data.Length, ipAddress, port);

        // Debug log to confirm the message sent
        //Debug.Log($"Sent data for {carId}: {message}");
    }

    void Update()
    {
        // Detect if 'P' key is pressed and toggle pause state
        bool p_pressed = Input.GetKeyDown(KeyCode.P);
        if (p_pressed && !old_p_pressed)
        {
            pauseSP = !pauseSP;
           // Debug.Log("Pause state toggled: " + pauseSP);
        }

        // Ensure nothing happens when paused
        if (!pauseSP)
        {
            // Send data for the yellow ghost car using its child's position and rotation
            if (yellowCarChild != null)
            {
                StartCoroutine(SendCarDataWithDelay("yellow", yellowCarChild.position, yellowCarChild.rotation.eulerAngles.y));
            }

            // Send data for the blue ghost car using its child's position and rotation
            if (blueCarChild != null)
            {
                StartCoroutine(SendCarDataWithDelay("blue", blueCarChild.position, blueCarChild.rotation.eulerAngles.y));
            }

            // Send data for the purple ghost car using its child's position and rotation
            if (purpleCarChild != null)
            {
                StartCoroutine(SendCarDataWithDelay("purple", purpleCarChild.position, purpleCarChild.rotation.eulerAngles.y));
            }
        }
        else
        {
         //   Debug.Log("Paused, no data sent.");
        }

        // Update the old state of 'P' key press
        old_p_pressed = p_pressed;
    }
}
