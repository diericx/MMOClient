using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemDataPanel_Controller : MonoBehaviour {

    public Text itemName;
    public Text itemData;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setItemName(string name)
    {
        itemName.text = name;
    }

    public void setItemData(string data)
    {
        itemData.text = data;
    }
}
