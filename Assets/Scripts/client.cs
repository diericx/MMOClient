using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using MsgPack.Serialization;

public class Client : MonoBehaviour
{

    ArrayList playerList = new ArrayList();
	ArrayList bulletList = new ArrayList();

    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private const float SERVER_SEND_RATE = 0.10f;

    public GameObject playerPrefab;
    public GameObject bulletPrefab;
    
    public int xMovement = 0;
    public int yMovement = 0;

    bool isAlive = true;

    private Thread _t1;

    // Use this for initialization
    void Start()
    {
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

        StartCoroutine(sendData());


    }

    // Update is called once per frame
    void Update()
    {
        getData();
    }

    public void sendInput(int x, int y)
    {
        //var serializer = MessagePackSerializer.Get<Packet>();

        //Packet p = new Packet();
        //p.Action = "input";
        //p.X = x;
        //p.Y = y;

        //var encodedMessage = serializer.PackSingleObject(p);

        ////			byte[] msgBytes = Encoding.UTF8.GetBytes(newJsonMsg.ToString()+"\n");
        //int i = server.Send(encodedMessage);
    }
    
    public void sendShot(float x, float y, int rotation) {
        //Debug.Log("SHOT");
        //BoxingPacker packer = new BoxingPacker();
		
        //Dictionary<string, object> message = new Dictionary<string, object>();
        //message.Add("Action", "shoot");
        //message.Add("ID", ((int)(LoginGUI.userID)).ToString());
        //message.Add("X", x);
        //message.Add("Y", y);
        //message.Add("Rotation", rotation);
		
        //var encodedMessage = packer.Pack(message);
		
        ////			byte[] msgBytes = Encoding.UTF8.GetBytes(newJsonMsg.ToString()+"\n");
        //int i = server.Send(encodedMessage);
	}
	
	//check if the player is already in the list
	public Player isPlayerAlreadyCreated(int id)
    {
        Player response = null;

        foreach(Player p in playerList )
        {
            if (p.id == id)
            {
                response = p;
            }
        }

        return response;
    }
    
	//check if the bullet is already in the list
	public Bullet isBulletAlreadyCreated(int id)
	{
		Bullet response = null;
		
		foreach(Bullet b in bulletList )
		{
			if (b.id == id)
			{
				response = b;
			}
		}
		
		return response;
	}

	//FUNCTUONS FOR INSTANTIATING GAME OBJECTS
    public GameObject instantiateNewPlayerObject()
    {
        GameObject playerObj = (GameObject)Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        return playerObj;
    }
    
	public GameObject instantiateNewBulletObject(float x, float y)
	{
		GameObject bulletObj = (GameObject)Instantiate(bulletPrefab, new Vector3(x, y, 0), Quaternion.identity);
		return bulletObj;
	}
	
	public void getData()
    {

        //while (isAlive)
        //{
            byte[] message = new byte[100];
            int available = server.Available;
            while (available >= 100)
            {
                server.Receive(message, 100, 0);

                var serializer = MessagePackSerializer.Get<Packet>();
                var deserializedObject = serializer.UnpackSingleObject(message);
                string action = deserializedObject.Action;
                print("action: " + action);

                if (action == "playerUpdate")
                {
                    //parse all variables
                    float x = deserializedObject.X;
                    float y = deserializedObject.Y;
                    int id = int.Parse(deserializedObject.ID);
                    //check if player is already created (already in the list)
                    Player foundPlayer = isPlayerAlreadyCreated(id);
                    print(id);

                    //if the player isn't already created
                    if (foundPlayer == null)
                    {
                        Debug.Log("Needs to be nade");
                        Player newPlayer = new Player(id);
                        newPlayer.playerObject = instantiateNewPlayerObject();
                        newPlayer.x = x;
                        newPlayer.y = y;
                        //add new object to player list
                        playerList.Add(newPlayer);

                    }
                    //if the player has already been created, edit it
                    else
                    {
                        Debug.Log("Player found");
                        foundPlayer.move(x, y);
                    }

                    //Debug.Log("Action: " + msg["Action"] + "; ID: " + msg["ID"] + "; X: " + msg["X"] + "; Y: " + msg["Y"]);
                }
                else if (action == "bulletUpdate")
                {
                    //parse all variables
                    print("BULLET UP");
                    float x = deserializedObject.X;
                    float y = deserializedObject.Y;
                    int id = int.Parse(deserializedObject.ID);
                    int rot = deserializedObject.Rotation;
                    //check if player is already created (already in the list)
                    Bullet foundBullet = isBulletAlreadyCreated(id);

                    //if the player isn't already created
                    if (foundBullet == null)
                    {
                        Debug.Log("Bullet needs to be made");
                        Bullet newBullet = new Bullet(id);
                        newBullet.bulletObject = instantiateNewBulletObject(x, y);
                        newBullet.move(x, y);
                        newBullet.bulletObject.transform.eulerAngles = new Vector3(0, 0, rot);
                        newBullet.x = x;
                        newBullet.y = y;
                        //add new object to player list
                        bulletList.Add(newBullet);

                    }
                    //if the bullet has already been created, edit it
                    else
                    {
                        Debug.Log("Bullet found");
                        foundBullet.move(x, y);
                    }
                }
                else if (action == "message")
                {
                    //Debug.Log("Action: " + deserializedObject.Action + "; Data: " + deserializedObject.Data);
                }
				
                available -= 100;
            }
			
			
			//yield return new WaitForSeconds(SERVER_GET_RATE);
        //}
	}
	
	IEnumerator sendData()
	{
		while (isAlive)
		{
            Debug.Log("SENDING");
            var serializer = MessagePackSerializer.Get<Packet>();

            var targetObject =
                new Packet
                {
                    Action = "update",
                    ID = ((int)(LoginGUI.userID)).ToString(),
                    X = xMovement,
                    Y = yMovement,
                    Rotation = 0,
                };

            targetObject.Tags.Add("Sample");
            targetObject.Tags.Add("Excellent");

            var encodedMessage = serializer.PackSingleObject(targetObject);
            //			byte[] msgBytes = Encoding.UTF8.GetBytes(newJsonMsg.ToString()+"\n");
            int i = server.Send(encodedMessage);

            string message = "";
            for (int j = 0; j < encodedMessage.Length; j++)
            {
                message += encodedMessage[j] + ", ";
            }

            print(message);

                Debug.Log("SENT");

            yield return new WaitForSeconds(SERVER_SEND_RATE);
        }
    }
}


public class Player {

    public int id;
    public float x;
    public float y;
    public GameObject playerObject;

    public Player(int id) {
        this.id = id;
        x = 0;
        y = 0;
    }

    public void move(float x, float y)
    {
        playerObject.GetComponent<Object_script>().setTargetPos(-99999, -99999);
        playerObject.transform.position = new Vector3(x, y, 0);
    }

}

public class Bullet {
	
	public int id;
	public float x;
	public float y;
	public GameObject bulletObject;
	
	public Bullet(int id) {
		this.id = id;
		x = 0;
		y = 0;
	}

    public Bullet()
    {
        id = -1;
        x = 0;
        y = 0;
    }
	
	public void move(float x, float y)
	{
		bulletObject.GetComponent<Object_script>().setTargetPos(x, y);
		//bulletObject.transform.position = new Vector3(x, y, 0);
	}
	
}
public class Packet
{
    public string Action { get; set; }
    public string ID { get; set; }
    public string Health { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public int Rotation { get; set; }
    public int[] Gear { get; set; }
    public bool IsNPC { get; set; }
    private readonly List<string> _tags = new List<string>();
    // Note that non-null read-only collection members are OK (of course, collections themselves must not be readonly.)
    public IList<string> Tags { get { return this._tags; } }
    //public Bullet[] Bullets { get; set; }
}
//public class Packet
//{
//    public string action { get; set; }
//    public string ID { get; set; }
//    public string health { get; set; }
//    public string x { get; set; }
//    public string y { get; set; }
//    public string rotation { get; set; }
//    public int[] gear { get; set; }
//    public bool isNPC { get; set; }
//    public Bullet[] bullets { get; set; }

//    public Packet() : base() { }

//}