using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

[Serializable]
public class AI_MESSAGE
{
    public string method;
    public string input;
    public string result;
}

public class YesNoClient : Singleton<YesNoClient>
{
    string IP_ADDRESS = "52.78.62.151";
    //string IP_ADDRESS = "192.168.0.124";
    int PORT = 1234;

    TcpClient tcpClient;
    NetworkStream networkStream;
    StreamWriter streamWriter;
    StreamReader streamReader;
    bool socketReady = false;

    byte[] sendByte;

    public void Open ()
    {
        try
        {
            tcpClient = new TcpClient(IP_ADDRESS, PORT);
            networkStream = tcpClient.GetStream();
            streamWriter = new StreamWriter(networkStream);
            streamReader = new StreamReader(networkStream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error:" + e);
        }
    }

    public void Close ()
    {
        if (!socketReady)
            return;
        streamWriter.Close();
        streamReader.Close();
        tcpClient.Close();
        socketReady = false;
    }

    public void Write (string theLine)
    {
        if (!socketReady)
            return;
    
        String tmpString = theLine + "\r\n";
        streamWriter.Write(tmpString);
        streamWriter.Flush();
    }

    public string Read ()
    {
        if (!socketReady)
            return "";
        if (networkStream.DataAvailable)
            return streamReader.ReadLine();
        return "";
    }
}
