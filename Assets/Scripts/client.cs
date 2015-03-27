using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using MsgPack;

public class client : MonoBehaviour
{

    ArrayList playerList = new ArrayList();

    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private const float SERVER_GET_RATE = 0.1f;
    private const float SERVER_SEND_RATE = 0.20f;

    public GameObject playerPrefab;
    public int xMovement = 0;
    public int yMovement = 0;

    bool isAlive = true;

    private Thread _t1;

    // Use this for initialization
    void Start()
    {
        server.SendTimeout = 5000;
        server.ReceiveTimeout = 5000;
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("67.160.192.45"), 7777);

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

        StartCoroutine(getData());

        StartCoroutine(sendData());


    }

    // Update is called once per frame
    void Update()
    {
        getData();
    }

    public void sendInput(int x, int y)
    {
        Debug.Log("INPUT");
        BoxingPacker packer = new BoxingPacker();

        Dictionary<string, object> message = new Dictionary<string, object>();
        message.Add("Action", "input");
        message.Add("X", x);
        message.Add("Y", y);

        var encodedMessage = packer.Pack(message);

        //			byte[] msgBytes = Encoding.UTF8.GetBytes(newJsonMsg.ToString()+"\n");
        int i = server.Send(encodedMessage);
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

    public GameObject instantiateNewPlayerObject()
    {
        GameObject playerObj = (GameObject)Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        return playerObj;
    }

    IEnumerator getData()
    {
        while (isAlive)
        {
            byte[] message = new byte[100];
            int available = server.Available;
            while (available >= 100)
            {
                server.Receive(message, 100, 0);

                BoxingPacker packer = new BoxingPacker();
                Dictionary<string, object> msg = (Dictionary<string, object>)packer.Unpack(message);
                string action = msg["Action"].ToString();

                if (action == "update")
                {
                    //parse all variables
                    float x = float.Parse(msg["X"].ToString());
                    float y = float.Parse(msg["Y"].ToString());
                    int id = int.Parse(msg["ID"].ToString());
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
                        Debug.Log("Player found");
                        foundPlayer.move(x, y);
                    }

                    Debug.Log("Action: " + msg["Action"] + "; ID: " + msg["ID"] + "; X: " + msg["X"] + "; Y: " + msg["Y"]);
                    //player.transform.position = new Vector3(x, y, 0);
                }
                else if (action == "message")
                {
                    Debug.Log("Action: " + msg["Action"] + "; Data: " + msg["Data"]);
                }

                available -= 100;
            }


            yield return new WaitForSeconds(SERVER_GET_RATE);
        }
    }

    IEnumerator sendData()
    {
        while (isAlive)
        {
            BoxingPacker packer = new BoxingPacker();

            Dictionary<string, object> message = new Dictionary<string, object>();
            message.Add("Action", "update");
            message.Add("ID", ((int)(LoginGUI.userID)).ToString());
            message.Add("X", xMovement);
            message.Add("Y", yMovement);
            message.Add("Rotation", 0);

            var encodedMessage = packer.Pack(message);

            //			byte[] msgBytes = Encoding.UTF8.GetBytes(newJsonMsg.ToString()+"\n");
            int i = server.Send(encodedMessage);

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
        playerObject.GetComponent<player_script>().setTargetPos(x, y);
        //playerObject.transform.position = new Vector3(x, y, 0);
    }

}