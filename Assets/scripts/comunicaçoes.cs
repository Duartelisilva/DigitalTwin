using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPSender : MonoBehaviour
{
    public string ipAddress = "127.0.0.1"; // IP address of the receiving computer
    public int port = 8085; // Port number of the receiving script

    private UdpClient udpClient;

    void Start()
    {
        udpClient = new UdpClient();
    }

    void OnDestroy()
    {
        udpClient.Close();
    }

    public void SendData(string carId, Vector3 position, float direction)
    {
        string message = $"{carId},{position.x},{position.y},{position.z},{direction}";
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, ipAddress, port);
    }

    // Example of how to use SendData
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SendData("yellow", new Vector3(-15f, 2.0f, -20), 45.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SendData("blue", new Vector3(-20f, 5.0f, -30f), 90.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SendData("purple", new Vector3(-30f, 8.0f, -30f), 135.0f);
        }
    }
}
