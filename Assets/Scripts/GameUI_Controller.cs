using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI_Controller : MonoBehaviour {
    public GameObject scrapsTxt;
	// Use this for initialization
	void Start () {
        scrapsTxt.GetComponent<RectTransform>().position = new Vector3(100, Screen.height - 100, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
