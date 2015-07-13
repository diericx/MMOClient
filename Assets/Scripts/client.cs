using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using MsgPack;

public class Client : MonoBehaviour
{

    ArrayList playerList = new ArrayList();
	ArrayList bulletList = new ArrayList();

    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private const float SERVER_SEND_RATE = 0.15f;
    private const float SERVER_GET_RATE = 0.10f;

    public GameObject playerPrefab;
    public GameObject bulletPrefab;
    public GameObject scrapsText;
    
    [HideInInspector]
    public int xMovement = 0;
    [HideInInspector]
    public int yMovement = 0;

    private long packetLength = -1;

    bool isAlive = true;

    private Thread _t1;

    // Use this for initialization
    void Start()
    {
        server.SendTimeout = 1000;
        server.ReceiveTimeout = 1000;
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(LoginGUI.ip), 7777);

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
        checkObjectsForExpiration();
    }

    
    public void sendShot(float x, float y, int rotation) {
		Debug.Log("SHOT");
		BoxingPacker packer = new BoxingPacker();
		
		Dictionary<string, object> message = new Dictionary<string, object>();
		message.Add("Action", "shoot");
		message.Add("ID", ((LoginGUI.userID)));
		message.Add("X", x);
		message.Add("Y", y);
		message.Add("Rotation", rotation);
		
		var encodedMessage = packer.Pack(message);

        server.Send(encodedMessage);

	}

    public void sendUpgradeRequest(string upgrade)
    {
        BoxingPacker packer = new BoxingPacker();
        Dictionary<string, object> message = new Dictionary<string, object>();
        message.Add("Action", "upgrade"+upgrade);

        var encodedMessage = packer.Pack(message);

        server.Send(encodedMessage);
    }

    public void sendJumpRequest(float x, float y)
    {
        BoxingPacker packer = new BoxingPacker();
        Dictionary<string, object> message = new Dictionary<string, object>();
        message.Add("Action", "jump");
        message.Add("X", x);
        message.Add("Y", y);

        var encodedMessage = packer.Pack(message);

        server.Send(encodedMessage);
    }

    //check if this is the main player
    public bool isThisTheClientPlayer(string id)
    {
        bool value = false;
        if (id.Equals(LoginGUI.userID) )
        {
            value = true;
        }
        return value;
    }
	
	//check if the player is already in the list
	public Player isPlayerAlreadyCreated(string id)
    {
        Player response = null;

        foreach(Player p in playerList )
        {
            if ( p.id.Equals(id) )
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

    void checkObjectsForExpiration()
    {
        foreach (Bullet b in bulletList)
        {
            float timeElapsedSinceLastUpdate = Time.time - b.lastUpdate;
            if (timeElapsedSinceLastUpdate >= 0.25)
            {
                UnityEngine.Object.Destroy(b.bulletObject);
                bulletList.Remove(b);
                break;
            }
        }

        foreach (Player p in playerList)
        {
            float timeElapsedSinceLastUpdate = Time.time - p.lastUpdate;
            if (timeElapsedSinceLastUpdate >= 1.5)
            {
                if (!isThisTheClientPlayer(p.id))
                {
                    UnityEngine.Object.Destroy(p.playerObject);
                    playerList.Remove(p);
                    break;
                }
            }
        }
    }

    void manageIncomingPlayerData(string id, float x, float y, int health)
    {
        //check if player is already created (already in the list)
        Player foundPlayer = isPlayerAlreadyCreated(id);
        //if the player isn't already created
        if (foundPlayer == null)
        {
            //add easy stuff
            Debug.Log("!!Player needs to be instantiated!!");
            Player newPlayer = new Player(id);
            newPlayer.playerObject = instantiateNewPlayerObject();
            newPlayer.x = x;
            newPlayer.y = y;
            newPlayer.health = health;
            newPlayer.lastUpdate = Time.time;
            //make camera follow this player if it is the clients player
            if (isThisTheClientPlayer(id))
            {
                Camera.main.transform.parent = newPlayer.playerObject.transform;
            }
            //add health bar
            Transform[] t = newPlayer.playerObject.GetComponentsInChildren<Transform>();
            newPlayer.healthBarObject = t[1].gameObject;
            t[2].GetComponent<TextMesh>().text = id;
            //add new object to player list
            playerList.Add(newPlayer);
            //print("Player created!");

        }
        //if the player has already been created, edit it
        else
        {
            foundPlayer.move(x, y);
            foundPlayer.updateHealth(health);
        }
    }

    void parsePacket(Dictionary<string, object> msg)
    {
        string action = msg["Action"].ToString();

        if (action == "playerUpdate")
        {
            //================PLAYER DATA======================
            //parse all variables
            float x = float.Parse(msg["X"].ToString());
            float y = float.Parse(msg["Y"].ToString());
            string id = (msg["ID"].ToString());
            int health = int.Parse(msg["Health"].ToString());
            int scraps = int.Parse(msg["Scraps"].ToString());
            //print("ID: " + id + ", X: " + x + ", Y: " + y);

            //get other player data arrays
            List<object> otherPlayerIDs = (List<object>)msg["OtherPlayerIDs"];
            List<object> otherPlayerXs = (List<object>)msg["OtherPlayerXs"];
            List<object> otherPlayerYs = (List<object>)msg["OtherPlayerYs"];
            List<object> otherPlayerHlths = (List<object>)msg["OtherPlayerHlths"];

            //update scraps text object
            scrapsText.GetComponent<Text>().text = "Scraps: " + scraps;

            //Manage clients player data
            manageIncomingPlayerData(id, x, y, health);

            //Manage other player data
            for (int i = 0; i < otherPlayerIDs.Count; i++)
            {
                //get data from array
                string otherPlayerID = (string)(otherPlayerIDs[i]);
                float otherPlayerX = float.Parse(otherPlayerXs[i].ToString());
                float otherPlayerY = float.Parse(otherPlayerYs[i].ToString());
                int otherPlayerHlth = int.Parse(otherPlayerHlths[i].ToString());
                //print(otherPlayerID + "; " + otherPlayerX + "; " + otherPlayerY);
                //manage the data
                manageIncomingPlayerData(otherPlayerID, otherPlayerX, otherPlayerY, otherPlayerHlth);
            }


            //================BULLET DATA======================
            //get bullet arrays
            List<object> bulletIDs = (List<object>)msg["BulletIDs"];
            List<object> bulletXs = (List<object>)msg["BulletXs"];
            List<object> bulletYs = (List<object>)msg["BulletYs"];
            List<object> bulletRots = (List<object>)msg["BulletRots"];

            for (int i = 0; i < bulletIDs.Count; i++)
            {
                //get data from arrays
                int bulletID = int.Parse(bulletIDs[i].ToString());
                float bulletX = float.Parse(bulletXs[i].ToString());
                float bulletY = float.Parse(bulletYs[i].ToString());
                int bulletRot = int.Parse(bulletRots[i].ToString());

                Bullet foundBullet = isBulletAlreadyCreated(bulletID);

                //if the player isn't already created
                if (foundBullet == null)
                {
                    Debug.Log("Bullet needs to be made");
                    Bullet newBullet = new Bullet(bulletID);
                    newBullet.bulletObject = instantiateNewBulletObject(bulletX, bulletY);
                    newBullet.move(bulletX, bulletY);
                    newBullet.bulletObject.transform.eulerAngles = new Vector3(0, 0, bulletRot);
                    newBullet.x = bulletX;
                    newBullet.y = bulletY;
                    //add new object to player list
                    bulletList.Add(newBullet);

                }
                //if the bullet has already been created, edit it
                else
                {
                    //Debug.Log("Bullet found");
                    foundBullet.move(bulletX, bulletY);
                }
            }


            //Debug.Log("Action: " + msg["Action"] + "; ID: " + msg["ID"] + "; X: " + msg["X"] + "; Y: " + msg["Y"]);
        }
        else if (action == "bulletUpdate")
        {
            //parse all variables
            float x = float.Parse(msg["X"].ToString());
            float y = float.Parse(msg["Y"].ToString());
            int id = int.Parse(msg["ID"].ToString());
            int rot = int.Parse(msg["Rotation"].ToString());
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
            Debug.Log("Action: " + msg["Action"] + "; Data: " + msg["Data"]);
        }
    }

    void getData()
    {
        while (server.Available > 0)
        {

            //if we havent gotten a packet header length yet
            if (packetLength == -1)
            {
                //get the packet header
                byte[] header = new byte[5];
                server.Receive(header, header.Length, 0);
                //turn bytes into string
                string headerString = System.Text.Encoding.Default.GetString(header);
                //get value of header string
                int headerVal = int.Parse(headerString, System.Globalization.NumberStyles.HexNumber);

                packetLength = headerVal;
            }
            else //if we already have a packet header length
            {
                int available = server.Available;
                //wait until the server loads that length
                if (available >= packetLength)
                {
                    //print("Packet Length: " + packetLength);
                    //load the packet
                    byte[] message = new byte[packetLength];
                    server.Receive(message, message.Length, 0);

                    //string messageString = System.Text.Encoding.Default.GetString(message);
                    //print("MESSAGE:" + messageString);

                    BoxingPacker packer = new BoxingPacker();
                    Dictionary<string, object> msg = (Dictionary<string, object>)packer.Unpack(message);

                    //---go through the packet and perform actions---
                    parsePacket(msg);

                    //reset packet length so that it knows to look for new headers
                    packetLength = -1;
                }

            }
        }

	}
	
	IEnumerator sendData()
	{
		while (isAlive)
		{
            //Debug.Log("SENDING");
            BoxingPacker packer = new BoxingPacker();

            Dictionary<string, object> message = new Dictionary<string, object>();
            message.Add("Action", "update");
            message.Add("ID", ((LoginGUI.userID)));
            message.Add("Username", "update");
            message.Add("X", xMovement);
            message.Add("Y", yMovement);
            message.Add("Rotation", 0);

            var encodedMessage = packer.Pack(message);

            server.Send(encodedMessage);

            yield return new WaitForSeconds(SERVER_SEND_RATE);
        }
    }
}


public class Player {

    public string id;
    public float x;
    public float y;
    public int health;
    public int scraps;
    public float lastUpdate;
    public GameObject playerObject;
    public GameObject healthBarObject;

    public Player(string id) {
        this.id = id;
        x = 0;
        y = 0;
    }

    public void move(float x, float y)
    {
        lastUpdate = Time.time;
        playerObject.GetComponent<Object_script>().setTargetPos(-99999, -99999);
        playerObject.transform.position = new Vector3(x, y, 0);
    }

    public void updateHealth(int health)
    {
        this.health = health;
        healthBarObject.transform.localScale = new Vector3( (float)health / 100f, healthBarObject.transform.localScale.y, healthBarObject.transform.localScale.z);
    }

    public void updateScraps(int scraps)
    {
        this.scraps = scraps;
    }

}

public class Bullet {
	
	public int id;
	public float x;
	public float y;
	public GameObject bulletObject;
    public float lastUpdate;
	
	public Bullet(int id) {
        lastUpdate = Time.time;
		this.id = id;
		x = 0;
		y = 0;
	}
	
	public void move(float x, float y)
	{
        lastUpdate = Time.time;
		bulletObject.GetComponent<Object_script>().setTargetPos(x, y);
		//bulletObject.transform.position = new Vector3(x, y, 0);
	}
	
}