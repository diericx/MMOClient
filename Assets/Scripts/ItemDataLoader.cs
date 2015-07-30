using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JSONEncoderDecoder;

public class ItemDataLoader
{
    private Hashtable items;
    private static ItemDataLoader instance;

    public static void Init()
    {
        if (instance == null)
        {
            instance = new ItemDataLoader();
            instance.items = (Hashtable)JSON.JsonDecode((Resources.Load("items") as TextAsset).text);
        }
    }


    public static Dictionary<string, int> getItemData(string itemID)
    {

        Dictionary<string, int> itemDataList = new Dictionary<string, int>();

        foreach (DictionaryEntry item in instance.items)
        {
            if (item.Key.Equals(itemID))
            {
                Hashtable itemData = (Hashtable)item.Value;
                //  ^this is each item
                //now have to loop through the item's table to get the stat changes
                foreach (DictionaryEntry data in itemData)
                {
                    itemDataList.Add(data.Key.ToString(), int.Parse(data.Value.ToString()));
                }
            }
        }

        return itemDataList;
    }

    public static int getItemAttribute(string itemID, string attr)
    {
        int value = 0;
        foreach (DictionaryEntry item in instance.items)
        {
            if (item.Key.Equals(itemID))
            {
                Hashtable itemData = (Hashtable)item.Value;
                //  ^this is each item
                //now have to loop through the item's table to get the stat changes
                foreach (DictionaryEntry data in itemData)
                {
                    //if this is the attribute we want
                    if (data.Key.ToString().Equals(attr))
                    {
                        value += int.Parse(data.Value.ToString());
                    }
                }
            }
        }
        return value;
    }
}
