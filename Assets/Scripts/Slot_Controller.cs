using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Slot_Controller : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
    public InventorySlot slot;
    private bool isInside = false;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (isInside && slot.item != "")
        {
            slot.updateDataPanelPosition();
            slot.getPanelObj().SetActive(true);
        }
        else
        {
            slot.getPanelObj().SetActive(false);
        }
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot.item != null && slot.item != "")
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                slot.clientScript.sendEquipRequest(slot.index);
            else if (eventData.button == PointerEventData.InputButton.Middle)
                Debug.Log("Middle click");
            else if (eventData.button == PointerEventData.InputButton.Right)
                slot.clientScript.sendDropRequest(slot.index);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isInside = false;
    }
}
