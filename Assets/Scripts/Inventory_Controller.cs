using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Inventory_Controller : MonoBehaviour {

    public Client clientScript;
    public GameObject slotPrefab;
    [HideInInspector]
    public List<InventorySlot> slotList;

    private List<object> inventoryTypes;
    private List<object> inventoryIDs;

	// Use this for initialization
	void Start () {
        slotList = new List<InventorySlot>();

        inventoryTypes = new List<object>();
        inventoryIDs = new List<object>();

        float xOffset = -60;
        float yOffset = 150;
        float xMargin = 120;
        float yMargin = -110;

        int i = 0;
        for (int col = 0; col < 2; col++)
        {
            for (int row = 0; row < 4; row++)
            {
                GameObject slotObj = (GameObject)Instantiate(slotPrefab);
                slotObj.transform.parent = this.gameObject.transform;
                slotObj.transform.localScale = new Vector3(1, 1, 1);
                slotObj.name = "slot" + col + "-" + row;
                slotObj.GetComponent<RectTransform>().localPosition = new Vector3(xOffset + (xMargin * col), yOffset + (yMargin * row));
                InventorySlot newSlot = new InventorySlot(i, slotObj, clientScript);
                slotList.Add(newSlot);
                slotObj.GetComponent<Slot_Controller>().slot = newSlot;
                i++;
            }
        }
	}

    public void updateInventory(List<object> inventoryTypes, List<object> inventoryIDs)
    {
        //print(inventoryIDs[0].ToString());
        if (!this.inventoryTypes.SequenceEqual(inventoryTypes) || !this.inventoryIDs.SequenceEqual(inventoryIDs))
        {
            this.inventoryTypes = inventoryTypes;
            this.inventoryIDs = inventoryIDs;

            //update slots
            for (int i = 0; i < slotList.Count; i++)
            {
                if (i <= inventoryTypes.Count - 1)
                {
                    slotList[i].updateSlot(inventoryTypes[i].ToString(), int.Parse(inventoryIDs[i].ToString()));
                }
                else
                {
                    slotList[i].updateSlot("", 0);
                }
                 
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class InventorySlot
{
    public Client clientScript;
    private GameObject slotObj;
    private GameObject slotIconObj;
    public int index;
    public string itemType;
    public int itemID;
    private Vector3 slotIconObjOffset;

    public InventorySlot(int index, GameObject slotObj, Client clientScript)
    {
        this.index = index;
        this.clientScript = clientScript;
        this.slotObj = slotObj;
        slotIconObjOffset = new Vector3(0, 0.12f, 0);
    }


    public void updateSlot(string type, int id)
    {
        if (type.Equals(""))
        {
            UnityEngine.Object.Destroy(slotIconObj);
            //update slot text
            Transform[] children = slotObj.GetComponentsInChildren<Transform>();
            children[1].GetComponent<Text>().text = "";
        }
        else
        {
            itemType = type;
            itemID = id;
            //remove icon if its already instantiated
            if (slotIconObj != null)
            {
                UnityEngine.Object.Destroy(slotIconObj);
            }
            //create new icon
            slotIconObj = PrefabLoader.Instantiate(type + id, slotObj.transform.position + slotIconObjOffset, Quaternion.identity);
            slotIconObj.transform.parent = slotObj.transform;
            slotIconObj.transform.localScale = new Vector3(12, 12, 12);
            //update slot text
            Transform[] children = slotObj.GetComponentsInChildren<Transform>();
            children[1].GetComponent<Text>().text = type;
            //update button script
            //slotObj.GetComponent<Button>().onClick.RemoveAllListeners();
            //slotObj.GetComponent<Button>().onClick.AddListener(() =>
            //{
            //    clientScript.sendEquipRequest(type, id);
            //});
        }


    }
}
