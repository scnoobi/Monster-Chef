using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    GameObject inventoryPanel;
    GameObject slotPanel;
    public GameObject inventorySlotPrefab;
    public GameObject inventoryItemPrefab;
    public const int SIZE_OF_INVENTORY = 64;
    public const int SIDE_OF_INVENTORY = 8;

    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();
    public List<bool> slotsEmpty = new List<bool>();

    public void Start()
    {
        inventoryPanel = this.gameObject;
        slotPanel = transform.GetChild(0).gameObject;

        for (int i = 0; i < SIZE_OF_INVENTORY; i++)
        {
            GameObject slot = (GameObject)Instantiate(inventorySlotPrefab);
            slots.Add(slot);
            slot.transform.SetParent(slotPanel.transform);
            slotsEmpty.Add(true);
        }
        this.gameObject.SetActive(false);
    }

    public bool isInventoryFull(int sizeX, int sizeY, ref int posEmpty)
    {
        if (items.Count == SIZE_OF_INVENTORY)
            return true;
        for (int i = 0; i < slotsEmpty.Count; i++)
        {
            if (slotsEmpty[i] && checkPositionsAreEmpty(sizeX, sizeY, i))
            {
                posEmpty = i;
                return false;
            }
        }
        return true;
    }

    public bool addItemToInventory(Sprite img, PickupItem PickupItem)
    {
        Item item = PickupItem.createItem();
        int posEmpty = -1;
        if (isInventoryFull(PickupItem.sizeX, PickupItem.sizeY, ref posEmpty))
            return false;

        items.Add(item);
        GameObject invItem = (GameObject)Instantiate(inventoryItemPrefab);
        invItem.GetComponent<ItemDraggable>().setItem(item);
        invItem.transform.SetParent(slots[MapGridToList(posEmpty, 0)].transform);
        invItem.transform.localPosition = Vector2.zero;
        invItem.GetComponent<Image>().sprite = img;
        for (int i = posEmpty; i < posEmpty + PickupItem.sizeX; i++)
        {
            for (int j = 0; j < PickupItem.sizeY; j++)
            {
                slots[MapGridToList(i, j)].GetComponent<Image>().color = Color.red;
                slotsEmpty[MapGridToList(i, j)] = false;
            }
        }
        return true;
    }

    public void check()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Debug.Log(MapGridToList(i, j));
            }
        }
    }

    public bool checkPositionsAreEmpty(int sizeX, int sizeY, int i)
    {
        int initI = i;
        bool isEmpty = true;
        for (; i < initI + sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                isEmpty &= checkPosOfGrid(i, j);
            }
        }
        return isEmpty;
    }

    public bool checkPosOfGrid(int i, int j)
    {
        return slotsEmpty[MapGridToList(i, j)];
    }

    public int MapGridToList(int i, int j)
    {
        return i + j * SIDE_OF_INVENTORY;
    }


}