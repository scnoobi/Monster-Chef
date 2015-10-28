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

    public bool addItemToInventory(PickupItem PickupItem)
    {
        Item item = PickupItem.createItem();
        int posEmpty = -1;
        if (isInventoryFull(PickupItem.sizeX, PickupItem.sizeY, ref posEmpty))
            return false;

        items.Add(item);
        GameObject invItem = (GameObject)Instantiate(inventoryItemPrefab);
        Image invItemImage = invItem.GetComponent<Image>();
        ItemDraggable draggable = invItem.GetComponent<ItemDraggable>();

        draggable.Initialize(this, item, PickupItem.sizeX, PickupItem.sizeY, PickupItem.GetComponent<SpriteRenderer>().sprite);
        draggable.slotId = slots[MapGridToList(posEmpty, 0)].GetComponent<Slots>().id;

        invItemImage.sprite = PickupItem.inventorySprite;

        invItem.transform.SetParent(slots[MapGridToList(posEmpty, 0)].transform);
        invItem.transform.localScale = new Vector3(PickupItem.sizeX, PickupItem.sizeY, 0);
        invItem.transform.localPosition = new Vector2(0, 0);
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
            isEmpty = true;
        }
        return isEmpty;
    }

    public int MapGridToList(int i, int j)
    {
        return i + j * SIDE_OF_INVENTORY;
    }

    /*
    public void OnDrawGizmos()
    {
        Vector3 size = new Vector3(5f, 5f, 5f);
        Gizmos.color = Color.blue;
        Vector3 sizeOfSlot = slots[0].transform.position - slots[SIDE_OF_INVENTORY+1].transform.position;
        for (int i = 0; i < slots.Count; i++)
        {
            Gizmos.DrawCube(slots[i].transform.position, new Vector3(5f, 5f, 5f));
        }
        Gizmos.color = Color.cyan;
        for (int i = 0; i < slots.Count; i++)
        {
            Gizmos.DrawCube(new Vector3(slots[i].transform.position.x - sizeOfSlot.x, slots[i].transform.position.y), new Vector3(5f, 5f, 5f));
            Gizmos.DrawCube(new Vector3(slots[i].transform.position.x, slots[i].transform.position.y - sizeOfSlot.y), new Vector3(5f, 5f, 5f));
        }
    }
    */

    public GameObject MapPositionToSlot(Vector3 position)
    {
        Vector3 sizeOfSlot = slots[0].transform.position - slots[SIDE_OF_INVENTORY + 1].transform.position;
        Vector3 slightNudge = sizeOfSlot / 2;
        position -= slightNudge; 
        Vector2 selectedSlot = new Vector2();
        for (int i = 0; i < SIDE_OF_INVENTORY; i++)
        {
            if (position.x < slots[i].transform.position.x) {
                selectedSlot.x = i - 1;
                break;
            }
        }

        for (int i = (int)selectedSlot.x; i < slots.Count; i += SIDE_OF_INVENTORY)
        {
           // Debug.Log(i + " " + position.y + " " + slots[i].transform.position.y);
            if (position.y > slots[i].transform.position.y)
            {
                selectedSlot.y = (i - SIDE_OF_INVENTORY) / SIDE_OF_INVENTORY;
                break;
            }
        }
        return slots[MapGridToList((int)selectedSlot.x, (int)selectedSlot.y)];
    }
}