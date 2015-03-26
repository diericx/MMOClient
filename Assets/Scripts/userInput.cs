using UnityEngine;
using System.Collections;

public class userInput : MonoBehaviour {
    public GameObject client;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey("w"))
        {
            client.GetComponent<client>().yMovement = 1;
        }
        else if (Input.GetKey("s"))
        {
            client.GetComponent<client>().yMovement = -1;
        }
        else
        {
            client.GetComponent<client>().yMovement = 0;
        }

        if (Input.GetKey("d"))
        {
             client.GetComponent<client>().xMovement = 1;
        }
        else if (Input.GetKey("a"))
        {
            client.GetComponent<client>().xMovement = -1;
        }
        else
        {
            client.GetComponent<client>().xMovement = 0;
        }
	}
}
