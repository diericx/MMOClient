using UnityEngine;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using MsgPack;
using MsgPack.Serialization;

public class Client2 : MonoBehaviour {

    TcpClient client;
    NetworkStream stream;
    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    int port = 7777;
    Thread _t1;
    Thread _t2;
    bool _t1Active = true;
    bool _t2Active = true;

    long packetLength = -1;

	// Use this for initialization
	void Start () {
        server.SendTimeout = 1000;
        server.ReceiveTimeout = 1000;
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("192.168.0.167"), 7777);

        try
        {
            server.Connect(ipep);
        }
        catch (SocketException e)
        {
            Debug.Log("Unable to connect to server!");
            Debug.Log(e.ToString());
            return;
        }

	}
	
	// Update is called once per frame
	void Update () {

        

        if (packetLength == -1)
        {
            byte[] header = new byte[8];
            server.Receive(header, header.Length, 0);

            string headerString = System.Text.Encoding.Default.GetString(header);
            print("HEADER:" + headerString);
            headerString = headerString.Substring(0, 8);
            long headerVal = Convert.ToInt64(headerString, 2);

            packetLength = headerVal;
        }
        else
        {
            int available = server.Available;

            if (available >= 32)
            {
                print("PACKET LENGTH: " + packetLength);
                byte[] message = new byte[32];
                server.Receive(message, 32, 0);

                string messageString = System.Text.Encoding.Default.GetString(message);
                print("MESSAGE:" + messageString);

                packetLength = -1;
            }


            //packetLength = -1;
        }

        //byte[] message = new byte[32];
        //server.Receive(message, message.Length, 0);
        //string messageString = System.Text.Encoding.Default.GetString(message);
        //print(message);

        //BoxingPacker packer = new BoxingPacker();
        //Dictionary<string, object> msg = (Dictionary<string, object>)packer.Unpack(message);
        //string action = msg["Action"].ToString();

        

        //BoxingPacker packer = new BoxingPacker();
        //var unpacked = (Dictionary<string, object>)packer.Unpack(stream);


	}

    void OnApplicationQuit()
    {
        _t1Active = false;
        _t2Active = false;
        client.Close();
        stream.Close();
    }
}