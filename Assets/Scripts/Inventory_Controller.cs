using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JSONEncoderDecoder;

public class Inventory_Controller : MonoBehaviour {

    public Client clientScript;
    public GameObject slotPrefab;
    public GameObject itemDataPanelPrefab;
    [HideInInspector]
    public List<InventorySlot> slotList;

    private List<object> inventory;

	// Use this for initialization
	void Start () {

        slotList = new List<InventorySlot>();

        inventory = new List<object>();

        float xOffset = -60;
        float yOffset = 150;
        float xMargin = 120;
        float yMargin = -110;

        int i = 0;
        for (int col = 0; col < 2; col++)
        {
            for (int row = 0; row < 4; row++)
            {
                //create slot object
                GameObject slotObj = (GameObject)Instantiate(slotPrefab);
                slotObj.transform.parent = this.gameObject.transform;
                slotObj.transform.localScale = new Vector3(1, 1, 1);
                slotObj.name = "slot" + col + "-" + row;
                slotObj.GetComponent<RectTransform>().localPosition = new Vector3(xOffset + (xMargin * col), yOffset + (yMargin * row));
                InventorySlot newSlot = new InventorySlot(i, slotObj, itemDataPanelPrefab, clientScript);
                slotList.Add(newSlot);
                slotObj.GetComponent<Slot_Controller>().slot = newSlot;
                i++;
            }
        }
	}

    public void updateInventory(List<object> inventory)
    {
        //print(inventoryIDs[0].ToString());
        if ( !this.inventory.SequenceEqual(inventory) )
        {
            this.inventory = inventory;

            //update slots
            for (int i = 0; i < slotList.Count; i++)
            {
                if (i <= inventory.Count - 1)
                {
                    slotList[i].updateSlot( inventory[i].ToString() );
                    slotList[i].updateItemPanel(inventory[i].ToString());
                }
                else
                {
                    slotList[i].updateSlot("");
                }
                 
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

	}
}

public class InventorySlot : MonoBehaviour
{
    public Client clientScript;
    private GameObject slotObj;
    private GameObject slotIconObj;
    private GameObject itemDataPanelObj;
    public int index;
    public string item;
    private Vector3 slotIconObjOffset;

    public InventorySlot(int index, GameObject slotObj, GameObject itemDataPanelPrefab, Client clientScript)
    {
        this.index = index;
        this.clientScript = clientScript;
        this.slotObj = slotObj;
        slotIconObjOffset = new Vector3(0, 0.12f, 0);

        itemDataPanelObj = (GameObject)Instantiate(itemDataPanelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        itemDataPanelObj.transform.localScale = new Vector3(1, 1, 1);
        itemDataPanelObj.transform.parent = GameObject.Find("Canvas").transform;
        itemDataPanelObj.SetActive(false);
    }

    public void updateDataPanelPosition()
    {
        RectTransform itemDataPanelRect = itemDataPanelObj.GetComponent<RectTransform>();

        float dataPanelW = itemDataPanelRect.sizeDelta.x;
        float dataPanelH = itemDataPanelRect.sizeDelta.y;

        Vector3 mouseOffSet = new Vector3(-dataPanelW / 2 - 10, -dataPanelH/2 - 10, 0);

        itemDataPanelObj.transform.localScale = new Vector3(1, 1, 1);
        var v3 = Input.mousePosition + mouseOffSet;
        v3.z = 10.0f;
        v3 = Camera.main.ScreenToWorldPoint(v3);

        Vector3 positionMove = new Vector3(v3.x, v3.y, v3.z );

        itemDataPanelRect.anchoredPosition = positionMove;
        itemDataPanelObj.transform.position = v3;
    }

    public void updateItemPanel(string itemId)
    {
        string itemDataString = "";
        //update panel title
        itemDataPanelObj.GetComponent<ItemDataPanel_Controller>().setItemName(itemId);

        //create item data string
        Dictionary<string, int> itemData = getItemData(itemId);
        foreach (KeyValuePair<string, int> data in itemData)
        {
            string key = data.Key.ToString();
            int val = (int)data.Value;
            print("Key: " + key);
            if (key == "healthCap")
            {
                itemDataString += "Health Cap + " + val.ToString();
            }
            else if (key == "speed")
            {
                itemDataString += "Speed + " + val.ToString();
            }
        }

        //update panel text
        itemDataPanelObj.GetComponent<ItemDataPanel_Controller>().setItemData(itemDataString);
    }

    public void updateSlot(string item)
    {
        if (item.Equals(""))
        {
            this.item = item;
            UnityEngine.Object.Destroy(slotIconObj);
            //update slot text
            foreach (Transform child in slotObj.transform)
            {
                if (child.name == "Text")
                {
                    child.GetComponent<Text>().text = "";
                }
            }
        }
        else
        {
            this.item = item;
            //remove icon if its already instantiated
            if (slotIconObj != null)
            {
                UnityEngine.Object.Destroy(slotIconObj);
            }
            //create new icon
            slotIconObj = PrefabLoader.Instantiate(item, slotObj.transform.position + slotIconObjOffset, Quaternion.identity);
            slotIconObj.transform.parent = slotObj.transform;
            slotIconObj.transform.localScale = new Vector3(12, 12, 12);
            //update slot text
            foreach (Transform child in slotObj.transform)
            {
                if (child.name == "Text")
                {
                    child.GetComponent<Text>().text = item;
                }
            }
            //update button script
            //slotObj.GetComponent<Button>().onClick.RemoveAllListeners();
            //slotObj.GetComponent<Button>().onClick.AddListener(() =>
            //{
            //    clientScript.sendEquipRequest(type, id);
            //});
        }


    }

    public Dictionary<string, int> getItemData(string itemID)
    {

        Dictionary<string, int> itemDataList = new Dictionary<string, int>();

        Hashtable items = (Hashtable)JSON.JsonDecode((Resources.Load("items") as TextAsset).text);
        foreach (DictionaryEntry item in items)
        {
            if (item.Key.Equals(itemID))
            {
                Hashtable itemData = (Hashtable)item.Value;
                //  ^this is each item
                //now have to loop through the item's table to get the stat changes
                foreach (DictionaryEntry data in itemData)
                {
                    print("     " + (string)data.Key + ": " + data.Value);
                    itemDataList.Add(data.Key.ToString(), int.Parse(data.Value.ToString()));
                }
            }
        }

        return itemDataList;
    }

    public GameObject getPanelObj()
    {
        return itemDataPanelObj;
    }
}

public class ItemData
{

    public int delta_healthCap;
    public int delta_speed;
    public ItemData()
    {

    }

}