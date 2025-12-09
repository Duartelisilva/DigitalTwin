using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPCommandSender : MonoBehaviour
{
    public string serverIP = "127.0.0.1"; // Change this to your server's IP address
    public int serverPort = 8083; // Default port number of the UDP server (the port Movimentomanual listens to)

    private UdpClient udpClient;

    void Start()
    {
        udpClient = new UdpClient();
    }

    void Update()
    {
        // Check for user input to switch ports
        if (Input.GetKey(KeyCode.Alpha1)) // Key '1'
        {
            serverPort = 8083;
           // Debug.Log("Switched to port: 8080");
        }
        else if (Input.GetKey(KeyCode.Alpha2)) // Key '2'
        {
            serverPort = 8081;
            //Debug.Log("Switched to port: 8081");
        }
        else if (Input.GetKey(KeyCode.Alpha3)) // Key '3'
        {
            serverPort = 8082;
            //Debug.Log("Switched to port: 8082");
        }

        // Check for movement commands and send corresponding UDP commands
        if (Input.GetKey(KeyCode.UpArrow))
        {
            SendCommand("mf"); // Move forward
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            SendCommand("mb"); // Move backward
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SendCommand("ml"); // Turn left
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SendCommand("mr"); // Turn right
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            SendCommand("rr"); // Reset steering
        }
        else if (Input.GetKeyDown(KeyCode.J) || Input.GetKey(KeyCode.KeypadPlus)) // '+' key
        {
            SendCommand("+d"); // Increase movement time
        }
        else if (Input.GetKeyDown(KeyCode.K) || Input.GetKey(KeyCode.KeypadMinus)) // '-' key
        {
            SendCommand("-d"); // Decrease movement time
        }
    }

    private void SendCommand(string command)
    {
        byte[] data = Encoding.UTF8.GetBytes(command);
        udpClient.Send(data, data.Length, serverIP, serverPort);
        //Debug.Log("Sent command: " + command + " to port: " + serverPort);
    }

    void OnApplicationQuit()
    {
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
}
