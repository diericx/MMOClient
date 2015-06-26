using UnityEngine;
using System.Collections;

public class userInput : MonoBehaviour {
    public Client clientScript;
    private float playerSpeed = 0f;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey("w"))
        {
			clientScript.yMovement = 1;
			Player p = clientScript.isPlayerAlreadyCreated((int)LoginGUI.userID);
			p.playerObject.transform.position = new Vector3(p.playerObject.transform.position.x, p.playerObject.transform.position.y + (1*playerSpeed), 0);
        }
        else if (Input.GetKey("s"))
        {
			clientScript.yMovement = -1;
			Player p = clientScript.isPlayerAlreadyCreated((int)LoginGUI.userID);
			p.playerObject.transform.position = new Vector3(p.playerObject.transform.position.x, p.playerObject.transform.position.y - (1*playerSpeed), 0);
        }
        else
        {
			clientScript.yMovement = 0;
        }

        if (Input.GetKey("d"))
        {
			clientScript.xMovement = 1;
			Player p = clientScript.isPlayerAlreadyCreated((int)LoginGUI.userID);
			p.playerObject.transform.position = new Vector3(p.playerObject.transform.position.x + (1*playerSpeed), p.playerObject.transform.position.y, 0);
        }
        else if (Input.GetKey("a"))
        {
			clientScript.xMovement = -1;
			Player p = clientScript.isPlayerAlreadyCreated((int)LoginGUI.userID);
			p.playerObject.transform.position = new Vector3(p.playerObject.transform.position.x - (1*playerSpeed), p.playerObject.transform.position.y, 0);
        }
        else
        {
			clientScript.xMovement = 0;
        }
        
		if (Input.GetMouseButtonDown(0)) {
			//get mouse world position
			float angleInDegrees;
			float mousex = (Input.mousePosition.x);
			float mousey = (Input.mousePosition.y);
			Vector3 mouseposition = Camera.main.ScreenToWorldPoint(new Vector3 (mousex,mousey,0));
			//get player object
			Player p = clientScript.isPlayerAlreadyCreated((int)(LoginGUI.userID));
			//if player is found, calculate angle
			if (p != null) {
				float deltaX = mouseposition.x - p.playerObject.transform.position.x;
				float deltaY = mouseposition.y - p.playerObject.transform.position.y;
				angleInDegrees = (Mathf.Atan2(deltaY, deltaX) * 180 / Mathf.PI) - 90;
				//print ((int)angleInDegrees);
				//send shot
				clientScript.sendShot(p.playerObject.transform.position.x, p.playerObject.transform.position.y, (int)angleInDegrees);
			}

			
		}
		
		
	}
}
