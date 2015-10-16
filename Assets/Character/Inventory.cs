using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {
    GameObject inventoryPanel;
    GameObject slotPanel;
    public GameObject inventorySlotPrefab;
    public GameObject inventoryItemPrefab;
    public int size = 47;

    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();

    void Start() {
        inventoryPanel = this.gameObject;
        slotPanel = transform.GetChild(0).gameObject;
        for (int i = 0; i < size; i++)
        {
            slots.Add(Instantiate(inventorySlotPrefab));
            slots[i].transform.SetParent(slotPanel.transform);
        }
    }

    public bool isInventoryFull()
    {
        return items.Count == size;
    }
}
