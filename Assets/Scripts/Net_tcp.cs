using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using MessagePack;
using System.Threading;
using System.Text;
using System.Linq;

public class Net_tcp : MonoBehaviour {

    const float SEND_REFRESH_RATE = 0.033f;

    public String host = "127.0.0.1";
    public Int32 port = 6666;
    public GameObject entityPrefab;
    public GameObject clientEntityPrefab;
	internal Boolean socket_ready = false;
    private int buffer_size = 2000;
    // Threading Vars
    private Thread receiveDataThread;
    private Queue<Entity> entityUpdateQueue = new Queue<Entity>();
    private int _entityUpdateQueueMax = 200;
    private object _entityUpdateQueueLock = new object();

    // Networking Vars 
	TcpClient tcp_socket;
    NetworkStream net_stream;

    public static Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

	private bool alive = true;
    private int clientID = -1;

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
        // apply scheduled entity updates
        manageEntityUpdateQueue();

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

    public static Entity getEntity(int id, bool isClient) {
        if (entities.ContainsKey(id)) {
            return entities[id];
        } else {
            return createEntity(id, isClient);
        }
    }

    public static void updateEntity(int id, Entity e) {
        if (entities.ContainsKey(id)) {
            Net_tcp.entities[id] = e;
        }
    }

    public Entity getClientEntity() {
        if (clientID != -1) {
            return getEntity(clientID, true);
        } else {
            return null;
        }
    }

    public static Entity createEntity(int id, bool isClient) {
        Entity newE = new Entity();
        newE.id = id;
        newE.isClient = isClient;
        entities[id] = newE;
        return newE;
    }
    
    // go through each enity object and ensure that there is a game object for it
    public void manageEntityGameObjects() {
        foreach(KeyValuePair<int, Entity> entry in entities)
        {
            Entity e = entry.Value;
            if (e.go == null) {
                GameObject prefab = null;

                if (e.isClient) {
                    prefab = clientEntityPrefab;
                    // set clientID var for this script to be used
                    clientID = e.id;
                    print("Instantiating client...");
                }  
                else {
                    prefab = entityPrefab;
                    print("Instantiating other player...");
                }

                e.go = GameObject.Instantiate(prefab, e.pos, Quaternion.identity);
                e.go.GetComponent<Entity_Script>().id = e.id;
                e.go.GetComponent<Entity_Script>().isClient = e.isClient;
            }
        }
    }

    // Goes through the update queue from received packets and applies them
    public void manageEntityUpdateQueue() {
        lock (_entityUpdateQueueLock)
         {
            while (entityUpdateQueue.Count > 0) {
                Entity e = entityUpdateQueue.Dequeue();
                entities[e.id] = e;
            }   
         }
    }

    public void scheduleEntityUpdate(Entity e) {
        lock (_entityUpdateQueueLock) {
            if (entityUpdateQueue.Count < _entityUpdateQueueMax) {
                entityUpdateQueue.Enqueue(e);
            }
        }
    }

    public void receiveData() {
        while (alive) {
            if (!socket_ready)
                continue;

            if (!net_stream.DataAvailable)
                continue;

            while (true) {
                try {
                    // ReceivePacket rp = readObject(net_stream);
                    ReceivePacket rp = MessagePackSerializer.Deserialize<ReceivePacket>(net_stream, true);
                    // ReceivePacket rp = MsgPack.Deserialize<ReceivePacket>(net_stream);
                    if (rp.type == "player") {
                        
                        Entity e = getEntity(rp.id, rp.is_client);
                        e.pos.x = (float)rp.x;
                        e.pos.y = (float)rp.y;
                        e.pos.z = (float)rp.z;
                        // schedule this update (so it happens in sync)
                        scheduleEntityUpdate(e);
                    }
                } catch (Exception e) {
                    print("Error parsing!: " + e.Message);
                    break;
                }
            }
        }
    }

    IEnumerator sendData() {
        while (alive) {
            Entity ce = getClientEntity();
            // if the client entity has actually been created
            if (ce != null) {
                // if the rotation has changed, send it!
                if (ce.rotChanged) {
                    RotationPacket rp = new RotationPacket() {
                        x = (double)ce.rot.x,
                        y = (double)ce.rot.y,
                        z = (double)ce.rot.z
                    };
                    var bytes = MessagePackSerializer.Serialize(rp);
                    writeSocket(bytes);
                }
            }
            yield return new WaitForSeconds(SEND_REFRESH_RATE);
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

    public void closeSocket()
    {
        if (!socket_ready)
            return;

        tcp_socket.Close();
        socket_ready = false;
    }

    public void cleanThreads() {
        receiveDataThread.Abort();
        alive = false;
    }

	public void setupSocket()
    {
        try
        {
            tcp_socket = new TcpClient(host, port);

            net_stream = tcp_socket.GetStream();

            // Start receiving data
            receiveDataThread = new Thread(receiveData);
            receiveDataThread.Start();
            // Start sending data
            StartCoroutine("sendData");

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
        cleanThreads();
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
public class RotationPacket
{
    [Key("type")]
    public string type = "rot";
    [Key("x")]
    public double x { get; set; }
    [Key("y")]
    public double y { get; set; }
    [Key("z")]
    public double z { get; set; }
}

[MessagePackObject]
public class ReceivePacket
{
    [Key("type")]
    public string type { get; set; }
    [Key("is_client")]
    public bool is_client { get; set; }
    [Key("id")]
    public int id { get; set; }
    [Key("x")]
    public double x { get; set; }
    [Key("y")]
    public double y { get; set; }
    [Key("z")]
    public double z { get; set; }
}

public class Entity {
    public int id;
    public bool isClient = false;
    public Vector3 pos;
    public Vector3 rot;
    public bool rotChanged = false;
    public GameObject go;
}