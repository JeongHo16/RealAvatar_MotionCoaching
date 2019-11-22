using UnityEngine;
using System.Net.Sockets;

public class SSH : MonoBehaviour
{
    const string IP = "52.78.62.151";
    const int PORT = 5001;
    public UdpClient udpClient = new UdpClient(IP, PORT);
}
