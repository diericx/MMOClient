using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Slot_Controller : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector]
    public InventorySlot slot;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot.itemType != null && slot.itemID != null)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                slot.clientScript.sendEquipRequest(slot.index);
            else if (eventData.button == PointerEventData.InputButton.Middle)
                Debug.Log("Middle click");
            else if (eventData.button == PointerEventData.InputButton.Right)
                slot.clientScript.sendDropRequest(slot.index);
        }
    }
}
