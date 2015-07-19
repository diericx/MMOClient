using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Net;
using SimpleJSON;

public class LoginGUI : MonoBehaviour {

	public static string userID = "-1";

    public GameObject errorTextGameObject;
    Text errorText;
    public Text ipText;
    public Text portText;
    public Text inputFieldTextObject;
    public Texture2D cursor;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    [HideInInspector]
    public static string username = "zac";
    [HideInInspector]
    public static string ip = "";
    [HideInInspector]
    public static string port = "";

	string usernameField = "Enter an ID";
	string passField = "xxxxxxxx";
	string baseURL = "";
	string errorString = "There was an error!";
	string successString = "Loading...";
	string errorLabelObjString = "";

	bool shouldGoToGame = false;

	// Use this for initialization
	void Start () {
        Cursor.SetCursor(cursor, hotSpot, cursorMode);

		baseURL = "http://" + "192.168.0.112" + ":3000/api/v1/";

        errorText = errorTextGameObject.GetComponent<Text>();

        ipText.text = "67.160.192.45";
        portText.text = "7777";
	}
	
	// Update is called once per frame
	void Update () {
		if (shouldGoToGame) {
			Application.LoadLevel ("GameScene");
		}
	}

	void login() {
		WebClient client = new WebClient ();
		client.DownloadStringCompleted += (s, e) => {
			if (e.Error == null && !e.Cancelled) {
				Debug.LogError (e.Result);
				var msg = JSON.Parse (e.Result);
				var id = msg["user_id"].Value;
				userID = id;
				shouldGoToGame = true;
				errorLabelObjString = successString;
			} else {
				errorLabelObjString = errorString;
				Debug.Log("ERROR: " + e.Error);
			}
			client.Dispose();
		};

		string url = baseURL + "login?username=" + usernameField + "&password=" + passField; 
		Debug.Log (url);

		client.DownloadStringAsync( new System.Uri( url ) );
	}

    public void enterGame()
    {
        //int parsedID = 0;
        //bool success = Int32.TryParse(inputFieldTextObject.text, out parsedID);
        //if (success == true)
        //{
        if (inputFieldTextObject.text != "")
        {
            userID = inputFieldTextObject.text;
            ip = ipText.text;
            port = portText.text;
            shouldGoToGame = true;
        }
        //}
        //else
        //{
        //    errorText.text = "Please enter ID as a valid number";
        //}
    }


	void OnGUI () {
		// // Make a background box
        //var loginBox = new Rect ();
        //loginBox.width = 200;
        //loginBox.height = 150;
        //loginBox.x = Screen.width / 2 - loginBox.width/2;
        //loginBox.y = Screen.height / 2 - loginBox.height/2;
        //GUI.Box(loginBox, "Login");

        //// Username field
        //var usernameFieldRect = new Rect ();
        //usernameFieldRect.width = 100;
        //usernameFieldRect.height = 20;
        //usernameFieldRect.x = loginBox.x + 50;
        //usernameFieldRect.y = loginBox.y + 30;
        //usernameField = GUI.TextField(usernameFieldRect, usernameField, 25);

		// password field
        //var passFieldRect = new Rect();
        //passFieldRect.width = 100;
        //passFieldRect.height = 20;
        //passFieldRect.x = loginBox.x + 50;
        //passFieldRect.y = usernameFieldRect.y + passFieldRect.height + 15;
        //passField = GUI.TextField(passFieldRect, passField, 25);

		// Make the second button.
        //var submitRect = new Rect ();
        //submitRect.width = 80;
        //submitRect.height = 20;
        //submitRect.x = (usernameFieldRect.x + (usernameFieldRect.width / 2)) - submitRect.width / 2;
        //submitRect.y = usernameFieldRect.y + 35;
        //if(GUI.Button(submitRect, "Submit")) {
        //    //login ();
        //    int parsedID = 0;
        //    bool success = Int32.TryParse(usernameField, out parsedID);
        //    if (success == true)
        //    {
        //        userID = parsedID;
        //        shouldGoToGame = true;
        //    } else {
        //        errorLabelObjString = "Please enter an integer ID";
        //    }
        //}

		// error text
        //var centeredStyle = GUI.skin.GetStyle("Label");
        //centeredStyle.alignment = TextAnchor.UpperCenter;
        //if (shouldGoToGame) {
        //    centeredStyle.normal.textColor = Color.green;
        //} else {
        //    centeredStyle.normal.textColor = Color.red;
        //}
        //GUI.Label(new Rect(loginBox.x, submitRect.y + 20 , loginBox.width, 20), errorLabelObjString, centeredStyle);

	}
}
