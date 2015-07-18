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
    ArrayList npcList = new ArrayList();
	ArrayList bulletList = new ArrayList();

    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private const float SERVER_SEND_RATE = 0.15f;
    private const float SERVER_GET_RATE = 0.10f;

    public GameUI_Controller GUIController;
    public GameObject playerPrefab;
    public GameObject bulletPrefab;
    public GameObject npcPrefab;
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

    //check if the npc is already in the list
    public Npc isNpcAlreadyCreated(int id)
    {
        Npc response = null;

        foreach (Npc n in npcList)
        {
            if (n.id == id)
            {
                response = n;
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

    public GameObject instantiateNewNpcObject(float x, float y)
    {
        GameObject npcObject = (GameObject)Instantiate(npcPrefab, new Vector3(x, y, 0), Quaternion.identity);
        return npcObject;
    }

    void checkObjectsForExpiration()
    {
        //Check bullets for expiration
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
        //Check players for expiration
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
        //Check npcs for expiration
        foreach (Npc npc in npcList)
        {
            float timeElapsedSinceLastUpdate = Time.time - npc.lastUpdate;
            if (timeElapsedSinceLastUpdate >= 0.25)
            {
                UnityEngine.Object.Destroy(npc.npcObject);
                npcList.Remove(npc);
                break;
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
            //add health bar
            Transform[] t = newPlayer.playerObject.GetComponentsInChildren<Transform>();
            newPlayer.healthBarObject = t[1].gameObject;
            t[2].GetComponent<TextMesh>().text = id;
            //make camera follow this player if it is the clients player
            if (isThisTheClientPlayer(id))
            {
                Camera.main.transform.parent = newPlayer.playerObject.transform;
                t[1].gameObject.SetActive(false);
            }
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

    void manageIncomingNpcData(int id, int type, float x, float y, int health)
    {
        Npc foundNpc = isNpcAlreadyCreated(id);

        //if the player isn't already created
        if (foundNpc == null)
        {
            Debug.Log("Bullet needs to be made");
            Npc newNpc = new Npc(id);
            newNpc.npcObject = instantiateNewNpcObject(x, y);
            newNpc.move(x, y);
            newNpc.npcObject.transform.eulerAngles = new Vector3(0, 0, 0);
            newNpc.x = x;
            newNpc.y = y;
            //add new object to player list
            npcList.Add(newNpc);

        }
        //if the bullet has already been created, edit it
        else
        {
            //Debug.Log("Bullet found");
            foundNpc.move(x, y);
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
            int healthCap = int.Parse(msg["HealthCap"].ToString());
            int energy = int.Parse(msg["Energy"].ToString());
            int energyCap = int.Parse(msg["EnergyCap"].ToString());
            int shield = int.Parse(msg["Shield"].ToString());
            int shieldCap = int.Parse(msg["ShieldCap"].ToString());
            int scraps = int.Parse(msg["Scraps"].ToString());
            int dmg = int.Parse(msg["Damage"].ToString());
            float speed = float.Parse(msg["Speed"].ToString());
            //print("ID: " + id + ", X: " + x + ", Y: " + y);

            //get other player data arrays
            List<object> otherPlayerIDs = (List<object>)msg["OtherPlayerIDs"];
            List<object> otherPlayerXs = (List<object>)msg["OtherPlayerXs"];
            List<object> otherPlayerYs = (List<object>)msg["OtherPlayerYs"];
            List<object> otherPlayerHlths = (List<object>)msg["OtherPlayerHlths"];

            //get bullet arrays
            List<object> bulletIDs = (List<object>)msg["BulletIDs"];
            List<object> bulletXs = (List<object>)msg["BulletXs"];
            List<object> bulletYs = (List<object>)msg["BulletYs"];
            List<object> bulletRots = (List<object>)msg["BulletRots"];

            //get NPC arrays
            List<object> npcIDs = (List<object>)msg["NpcIDs"];
            List<object> npcTypes = (List<object>)msg["NpcTypes"];
            List<object> npcXs = (List<object>)msg["NpcXs"];
            List<object> npcYs = (List<object>)msg["NpcYs"];
            List<object> npcHlths = (List<object>)msg["NpcHlths"];

            //update hud scraps text
            GUIController.updateScrapsTxt(scraps);

            //update dmg text
            GUIController.updateDmgTxt(dmg);

            //update hud health bar
            GUIController.updateCurrentHealth(health, healthCap);

            //update energy hud bar
            GUIController.updateCurrentEnergy(energy, energyCap);

            //update shield bars
            GUIController.updateCurrentShield(shield, shieldCap);
            
            //update hud speed
            GUIController.updateCurrentSpeed(speed);

            //Manage clients player data
            manageIncomingPlayerData(id, x, y, health);

            //================Other Player DATA======================
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


            //================NPC DATA======================

            for (int i = 0; i < npcIDs.Count; i++)
            {
                //get data from arrays
                int npcID = int.Parse(npcIDs[i].ToString());
                int npcType = int.Parse(npcTypes[i].ToString());
                float npcX = float.Parse(npcXs[i].ToString());
                float npcY = float.Parse(npcYs[i].ToString());
                int npcHlth = int.Parse(npcHlths[i].ToString());

                manageIncomingNpcData(npcID, npcType, npcX, npcY, npcHlth);
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
            int angleInDegrees = 0;

            //get mouse world positon
            Vector3 screenPoint = Input.mousePosition;
            screenPoint.z = 20.0f; //distance of the plane from the camera
            var mouseposition = Camera.main.ScreenToWorldPoint(screenPoint);
            //get player object
            Player p = isPlayerAlreadyCreated((LoginGUI.userID));
            //if player is found, calculate angle
            if (p != null)
            {
                float deltaX = mouseposition.x - p.playerObject.transform.position.x;
                float deltaY = mouseposition.y - p.playerObject.transform.position.y;
                float angle = (Mathf.Atan2(deltaY, deltaX) * 180 / Mathf.PI) - 90;
                angleInDegrees = Convert.ToInt32(angle);
            }

            Dictionary<string, object> message = new Dictionary<string, object>();
            message.Add("Action", "update");
            message.Add("ID", ((LoginGUI.userID)));
            message.Add("Shooting", Input.GetMouseButton(0));
            //message.Add("Username", "update");
            message.Add("X", xMovement);
            message.Add("Y", yMovement);
            message.Add("Rotation", angleInDegrees);

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


public class Npc
{

    public int id;
    public int type;
    public float x;
    public float y;
    public GameObject npcObject;
    public float lastUpdate;

    public Npc(int id)
    {
        lastUpdate = Time.time;
        this.id = id;
        x = 0;
        y = 0;
    }

    public void move(float x, float y)
    {
        lastUpdate = Time.time;
        npcObject.GetComponent<Object_script>().setTargetPos(x, y);
        //bulletObject.transform.position = new Vector3(x, y, 0);
    }

}