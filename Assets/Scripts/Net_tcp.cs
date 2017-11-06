using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using MessagePack;
using System.Threading;

public class Net_tcp : MonoBehaviour {

    public String host = "127.0.0.1";
    public Int32 port = 6666;
    public GameObject entityPrefab;
	internal Boolean socket_ready = false;
	internal String input_buffer = "";
    private int buffer_size = 1000;
    private Thread receiveDataThread;

	TcpClient tcp_socket;
    NetworkStream net_stream;
    StreamWriter socket_writer;
    StreamReader socket_reader;

    public static Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

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
        // update entity game objects
        manageEntityGameObjects();

        // Sends user inputs to the server as they happen
        string[] keys = {"w", "a", "s", "d"};
        handleUserInput( keys );

	}

    public void handleUserInput(string[] keys) {
        InputPacket packet = new InputPacket();

        bool shouldSend = false;
        foreach (string k in keys) {
            if (Input.GetKeyDown(k) || Input.GetKeyUp(k)) {
                shouldSend = true;
                break;
            }
        }

        if (!shouldSend)
            return;

        packet.w = Input.GetKey("w");
        packet.a = Input.GetKey("a");
        packet.s = Input.GetKey("s");
        packet.d = Input.GetKey("d");
        
        var bytes = MessagePackSerializer.Serialize(packet);
        writeSocket(bytes);
    }

    public static Entity getEntity(int id) {
        if (entities.ContainsKey(id)) {
            return entities[id];
        } else {
            return createEntity(id);
        }
    }

    public static Entity createEntity(int id) {
        Entity newE = new Entity();
        entities[id] = newE;
        return newE;
    }
    
    // go through each enity object and ensure that there is a game object for it
    public void manageEntityGameObjects() {
        foreach(KeyValuePair<int, Entity> entry in entities)
        {
            Entity e = entry.Value;
            if (e.go == null) {
                e.go = GameObject.Instantiate(entityPrefab, e.pos, Quaternion.identity);
                e.go.GetComponent<Entity_Script>().id = e.id;
            }
        }
    }

    public void receiveData() {
        print("startig to listen...");
        while (alive) {
            if (socket_ready) {
                byte[] buffer = new byte[buffer_size];
                net_stream.Read(buffer, 0, buffer_size);

                try {
                    ReceivePacket rp =  MessagePackSerializer.Deserialize<ReceivePacket>(buffer);
                    if (rp.type == "player") {
                        Entity e = getEntity(rp.id);
                        e.pos.x = rp.x;
                        e.pos.y = rp.y;
                        e.pos.z = rp.z;
                        entities[e.id] = e;
                    }
                } catch (Exception e) {
                    print("Error parsing!: " + e.Message);
                }

            }
        }
    }

	public void writeSocket(byte[] line)
    {
        if (!socket_ready)
            return;

		net_stream.Write(line, 0, line.Length);
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

            // Start receiving data
            receiveDataThread = new Thread(receiveData);
            receiveDataThread.Start();

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
        receiveDataThread.Abort();
    }
}

[MessagePackObject]
public class InputPacket
{
    [Key("type")]
    public string type = "input";
    [Key("w")]
    public bool w { get; set; }
    [Key("a")]
    public bool a { get; set; }
    [Key("s")]
    public bool s { get; set; }
    [Key("d")]
    public bool d { get; set; }
}

[MessagePackObject]
public class ReceivePacket
{
    [Key("type")]
    public string type { get; set; }
    [Key("id")]
    public int id { get; set; }
    [Key("x")]
    public int x { get; set; }
    [Key("y")]
    public int y { get; set; }
    [Key("z")]
    public int z { get; set; }
}

public class Entity {
    public int id;
    public Vector3 pos;
    public GameObject go;
}