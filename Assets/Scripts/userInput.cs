using UnityEngine;
using System.Collections;
using System;

public class userInput : MonoBehaviour {
    public Client clientScript;
    public GameObject upgradeGUI;
    public GameObject inventoryGUI;
    private float playerSpeed = 0f;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        int angleInDegrees = 0;

        //get mouse world positon
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 20.0f; //distance of the plane from the camera
        var mouseposition = Camera.main.ScreenToWorldPoint(screenPoint);
        //get player object
        Player p = clientScript.isPlayerAlreadyCreated((LoginGUI.userID));
        //if player is found, calculate angle
        if (p != null)
        {
            float deltaX = mouseposition.x - p.playerObject.transform.position.x;
            float deltaY = mouseposition.y - p.playerObject.transform.position.y;
            float angle = (Mathf.Atan2(deltaY, deltaX) * 180 / Mathf.PI) - 90;
            angleInDegrees = Convert.ToInt32(angle);
            //p.rotate(angleInDegrees);
        }

        if (Input.GetKey("w"))
        {
			clientScript.yMovement = 1;
			p.playerObject.transform.position = new Vector3(p.playerObject.transform.position.x, p.playerObject.transform.position.y + (1*playerSpeed), 0);
        }
        else if (Input.GetKey("s"))
        {
			clientScript.yMovement = -1;
			p.playerObject.transform.position = new Vector3(p.playerObject.transform.position.x, p.playerObject.transform.position.y - (1*playerSpeed), 0);
        }
        else
        {
			clientScript.yMovement = 0;
        }

        if (Input.GetKey("d"))
        {
			clientScript.xMovement = 1;
			p.playerObject.transform.position = new Vector3(p.playerObject.transform.position.x + (1*playerSpeed), p.playerObject.transform.position.y, 0);
        }
        else if (Input.GetKey("a"))
        {
			clientScript.xMovement = -1;
			p.playerObject.transform.position = new Vector3(p.playerObject.transform.position.x - (1*playerSpeed), p.playerObject.transform.position.y, 0);
        }
        else
        {
			clientScript.xMovement = 0;
        }

        //Upgrade buttons
        if (Input.GetKeyDown("q"))
        {
            upgradeGUI.SetActive(!upgradeGUI.activeSelf);
        }
        if (Input.GetKeyDown("i"))
        {
            inventoryGUI.SetActive(!inventoryGUI.activeSelf);
        }


        if (Input.GetKeyDown("space"))
        {
            clientScript.sendJumpRequest(mouseposition.x, mouseposition.y);
        }
		
	}
}
