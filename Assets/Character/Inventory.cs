using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {
    GameObject inventoryPanel;
    GameObject slotPanel;
    public GameObject inventorySlotPrefab;
    public GameObject inventoryItemPrefab;
    public int size = 48;

    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();
    public List<bool> slotsFilled = new List<bool>();

    void Start() {
        Debug.Log(inventoryItemPrefab.name);
        Debug.Log(inventorySlotPrefab.name);
        inventoryPanel = this.gameObject;
        slotPanel = transform.GetChild(0).gameObject;
        
        for (int i = 0; i < size; i++)
        {
            GameObject slot = (GameObject)Instantiate(inventorySlotPrefab);
            slots.Add(slot);
            slot.transform.SetParent(slotPanel.transform);
            slotsFilled.Add(false);
        }
    }

    public bool isInventoryFull()
    {
        return items.Count == size;
    }

    public void addItemToInventory(Sprite img, Item item) {
        items.Add(item);
        GameObject invItem = (GameObject)Instantiate(inventoryItemPrefab);
        for (int i = 0; i < size; i++)
        {
            if (!slotsFilled[i])
            {
                invItem.transform.SetParent(slots[i].transform);
                invItem.transform.localPosition = Vector2.zero;
                Debug.Log(invItem.transform.parent.name);
                slotsFilled[i] = true;
                break;
            }
        }
    }



}
