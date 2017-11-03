using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using MessagePack;

public class Net_tcp : MonoBehaviour {

    public String host = "127.0.0.1";
    public Int32 port = 6666;
	internal Boolean socket_ready = false;
	internal String input_buffer = "";

	TcpClient tcp_socket;
    NetworkStream net_stream;
    StreamWriter socket_writer;
    StreamReader socket_reader;


	private bool alive = true;

	// Use this for initialization
	void Start () {

	}

	void Awake()
    {
        setupSocket();
    }
	
	// Update is called once per frame
	void Update () {
		string received_data = readSocket();
        string key_stroke = Input.inputString;

        // Collects keystrokes into a buffer
        if (key_stroke != ""){
            input_buffer += key_stroke;

            if (key_stroke == "\n"){
            	// Send the buffer, clean it
            	Debug.Log("Sending: " + input_buffer);
            	writeSocket(input_buffer);
            	input_buffer = "";
            }

        }


        if (received_data != "")
        {
        	// Do something with the received data,
        	// print it in the log for now
            Debug.Log(received_data);
        }
	}

	public void writeSocket(string line)
    {
        if (!socket_ready)
            return;

		var samplePacket = new Sample1
        {
            X = 2,
            Y = 2
        };
        var bytes = MessagePackSerializer.Serialize(samplePacket);
		net_stream.Write(bytes, 0, bytes.Length);
        // line = line + "\r\n";
		// Byte[] data = System.Text.Encoding.ASCII.GetBytes("hi");
		// 
		// MessagePack.Serialize(new { field1 = 1, field2 = 2 }, net_stream);
    }

    public String readSocket()
    {
        if (!socket_ready)
            return "";

        if (net_stream.DataAvailable)
            return socket_reader.ReadLine();

        return "";
    }

    public void closeSocket()
    {
        if (!socket_ready)
            return;

        socket_writer.Close();
        socket_reader.Close();
        tcp_socket.Close();
        socket_ready = false;
    }

	public void setupSocket()
    {
        try
        {
            tcp_socket = new TcpClient(host, port);

            net_stream = tcp_socket.GetStream();
            socket_writer = new StreamWriter(net_stream);
            socket_reader = new StreamReader(net_stream);

            socket_ready = true;
        }
        catch (Exception e)
        {
        	// Something went wrong
            Debug.Log("Socket error: " + e);
        }
    }

	void OnApplicationQuit()
    {
        closeSocket();
    }
}

[MessagePackObject]
public class Sample1
{
    [Key("X")]
    public int X { get; set; }
    [Key("Y")]
    public int Y { get; set; }
}