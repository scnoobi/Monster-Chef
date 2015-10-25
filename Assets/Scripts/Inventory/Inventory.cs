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
            slot.transform.localScale = slotPanel.transform.localScale;
            slot.GetComponent<Slots>().id = i;
            slotsEmpty.Add(true);

        }
        /*
        int i = 0;
        foreach (Transform slot in transform.GetChild(0)) {
            slots.Add(slot.gameObject);
            slot.GetComponent<Slots>().id = i;
            slotsEmpty.Add(true);
            i++;
        }
        */
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
        ItemDraggable draggable = invItem.GetComponent<ItemDraggable>();
        draggable.setItem(item);
        draggable.setSize(PickupItem.sizeX, PickupItem.sizeY);
        invItem.transform.SetParent(slots[MapGridToList(posEmpty, 0)].transform);
        invItem.transform.localPosition = Vector2.zero;
        invItem.transform.localScale = invItem.transform.parent.localScale;
        invItem.GetComponent<Image>().sprite = img;
        occupyGridWithItem(PickupItem.sizeX, PickupItem.sizeY, posEmpty, false, draggable);
        return true;
    }

    public void occupyGridWithItem(int sizeX, int sizeY, int startPos, bool empty, ItemDraggable itemDraggable)
    {
        Color colorToPaint;
        for (int i = startPos; i < startPos + sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                int index = MapGridToList(i, j);
                if (empty)
                    colorToPaint = Color.white;
                else
                    colorToPaint = Color.red;
                slots[index].GetComponent<Image>().color = colorToPaint;
                slots[index].GetComponent<Slots>().droppedItem = itemDraggable;
                slotsEmpty[index] = empty;
            }
        }
    }

    public void check()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Debug.Log(slotsEmpty[MapGridToList(i, j)]);
            }
        }
    }

    public bool checkPositionsAreEmpty(int sizeX, int sizeY, int i, ItemDraggable itemDraggable = null)
    {
        int initI = i;
        bool isEmpty = true;
        for (; i < initI + sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                if (MapGridToList(i, j) >= SIZE_OF_INVENTORY)
                    return false;
                isEmpty &= checkPosOfGrid(i, j, itemDraggable);
            }
        }
        return isEmpty;
    }

    public bool checkPosOfGrid(int i, int j, ItemDraggable itemDraggable)
    {
        int index = MapGridToList(i, j);
        bool isEmpty = slotsEmpty[index];
        if (itemDraggable != null && !isEmpty && slots[index].GetComponent<Slots>().droppedItem.GetInstanceID() == itemDraggable.GetInstanceID())
        {
            Debug.Log(itemDraggable.GetInstanceID() + " dasd " + slots[index].GetComponent<Slots>().droppedItem.GetInstanceID());
            isEmpty = true;
        }
        return isEmpty;
    }

    public int MapGridToList(int i, int j)
    {
        return i + j * SIDE_OF_INVENTORY;
    }


}