﻿using UnityEngine;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using MsgPack;
using MsgPack.Serialization;

public class Client : MonoBehaviour
{

    ArrayList playerList = new ArrayList();
	ArrayList bulletList = new ArrayList();

    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private const float SERVER_SEND_RATE = 0.15f;
    private const float SERVER_GET_RATE = 0.10f;

    public GameObject playerPrefab;
    public GameObject bulletPrefab;
    
    public int xMovement = 0;
    public int yMovement = 0;

    private long packetLength = -1;

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

    
    public void sendShot(float x, float y, int rotation) {
		Debug.Log("SHOT");
		BoxingPacker packer = new BoxingPacker();
		
		Dictionary<string, object> message = new Dictionary<string, object>();
		message.Add("Action", "shoot");
		message.Add("ID", ((int)(LoginGUI.userID)).ToString());
		message.Add("X", x);
		message.Add("Y", y);
		message.Add("Rotation", rotation);
		
		var encodedMessage = packer.Pack(message);

        server.Send(encodedMessage);

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

    void parsePacket(Dictionary<string, object> msg)
    {
        string action = msg["Action"].ToString();

        if (action == "playerUpdate")
        {
            //================PLAYER DATA======================
            //parse all variables
            float x = float.Parse(msg["X"].ToString());
            float y = float.Parse(msg["Y"].ToString());
            int id = int.Parse(msg["ID"].ToString());
            //int[] bulletIDs = (int[])msg["BulletIDs"];
            //Dictionary<string, object> bullets = (Dictionary<string, object>)msg["Bullets"];

            //check if player is already created (already in the list)
            Player foundPlayer = isPlayerAlreadyCreated(id);

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
                foundPlayer.move(x, y);
            }
            //================BULLET DATA======================
            //get bullet arrays
            print(msg["BulletIDs"]);
            List<object> bulletIDs = (List<object>)msg["BulletIDs"];
            List<object> bulletXs = (List<object>)msg["BulletXs"];
            List<object> bulletYs = (List<object>)msg["BulletYs"];
            List<object> bulletRots = (List<object>)msg["BulletRots"];
            
            for (int i = 0; i < bulletIDs.Count; i++)
            {
                //get data from arrays
                int bulletID = Convert.ToInt32(bulletIDs[i]);
                float bulletX = Convert.ToSingle(bulletXs[i]);
                float bulletY = Convert.ToSingle(bulletYs[i]);
                int bulletRot = Convert.ToInt32(bulletRots[i]);
                print(bulletID);

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
                    Debug.Log("Bullet found");
                    foundBullet.move(bulletX, bulletY);
                }
            }


            //Debug.Log("Action: " + msg["Action"] + "; ID: " + msg["ID"] + "; X: " + msg["X"] + "; Y: " + msg["Y"]);
        }
        else if (action == "bulletUpdate")
        {
            //parse all variables
            print("BULLET UP");
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
                print("Packet Length: " + packetLength);
                //load the packet
                byte[] message = new byte[packetLength];
                server.Receive(message, message.Length, 0);

                string messageString = System.Text.Encoding.Default.GetString(message);
                //print("MESSAGE:" + messageString);

                BoxingPacker packer = new BoxingPacker();
                Dictionary<string, object> msg = (Dictionary<string, object>)packer.Unpack(message);
                string action = msg["Action"].ToString();


                //---go through the packet and perform actions---
                parsePacket(msg);

                //reset packet length so that it knows to look for new headers
                packetLength = -1;
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
            message.Add("ID", ((int)(LoginGUI.userID)).ToString());
            message.Add("X", xMovement);
            message.Add("Y", yMovement);
            message.Add("Rotation", 0);

            var encodedMessage = packer.Pack(message);

            ////			byte[] msgBytes = Encoding.UTF8.GetBytes(newJsonMsg.ToString()+"\n");
            int i = server.Send(encodedMessage);

            //var targetObject =
            //    new UpdatePacket
            //    {
            //        Action = "update",
            //        ID = ((int)(LoginGUI.userID)).ToString(),
            //        X = xMovement,
            //        Y = yMovement,
            //        Rotation = 0,
            //    };

            //if (stream != null)
            //{
                //serializer.Pack(stream, targetObject);
                //byte[] packet = serializer.PackSingleObject(targetObject);
                //server.Send(packet);
                //stream.Write(packet, 0, packet.Length);
            //}
            
            //Debug.Log("SENT");

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
	
	public void move(float x, float y)
	{
		bulletObject.GetComponent<Object_script>().setTargetPos(x, y);
		//bulletObject.transform.position = new Vector3(x, y, 0);
	}
	
}