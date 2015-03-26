using UnityEngine;
using System.Collections;
using System.Net;
using SimpleJSON;

public class LoginGUI : MonoBehaviour {

	public static float userID = -1;

	string usernameField = "Zac";
	string passField = "xxxxxxxx";
	string baseURL = "";
	string errorString = "There was an error!";
	string successString = "Loading...";
	string errorLabelObjString = "";

	bool shouldGoToGame = false;

	// Use this for initialization
	void Start () {
		baseURL = "http://" + "192.168.0.112" + ":3000/api/v1/";
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
				userID = int.Parse(id);
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


	void OnGUI () {
		// Make a background box
		var loginBox = new Rect ();
		loginBox.width = 200;
		loginBox.height = 150;
		loginBox.x = Screen.width / 2 - loginBox.width/2;
		loginBox.y = Screen.height / 2 - loginBox.height/2;
		GUI.Box(loginBox, "Login");

		// Username field
		var usernameFieldRect = new Rect ();
		usernameFieldRect.width = 100;
		usernameFieldRect.height = 20;
		usernameFieldRect.x = loginBox.x + 50;
		usernameFieldRect.y = loginBox.y + 30;
		usernameField = GUI.TextField(usernameFieldRect, usernameField, 25);

		// password field
		var passFieldRect = new Rect ();
		passFieldRect.width = 100;
		passFieldRect.height = 20;
		passFieldRect.x = loginBox.x + 50;
		passFieldRect.y = usernameFieldRect.y + passFieldRect.height + 15;
		passField = GUI.TextField(passFieldRect, passField, 25);

		// Make the second button.
		var submitRect = new Rect ();
		submitRect.width = 80;
		submitRect.height = 20;
		submitRect.x = (passFieldRect.x + (passFieldRect.width/2)) - submitRect.width/2 ;
		submitRect.y = passFieldRect.y + 35;
		if(GUI.Button(submitRect, "Submit")) {
			login ();
		}

		// error text
		var centeredStyle = GUI.skin.GetStyle("Label");
		centeredStyle.alignment = TextAnchor.UpperCenter;
		if (shouldGoToGame) {
			centeredStyle.normal.textColor = Color.green;
		} else {
			centeredStyle.normal.textColor = Color.red;
		}
		GUI.Label(new Rect(loginBox.x, submitRect.y + 20 , loginBox.width, 20), errorLabelObjString, centeredStyle);

	}
}
