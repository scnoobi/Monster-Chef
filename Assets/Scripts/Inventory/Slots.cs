using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Slots : MonoBehaviour, IDropHandler {

    private Inventory inv;
    public int id;
    public ItemDraggable droppedItem;

	void Start () {
        inv = GetComponentInParent<Inventory>();
	}

    public void OnDrop(PointerEventData eventData)
    {
        ItemDraggable droppedItem = eventData.pointerDrag.GetComponent<ItemDraggable>();
        bool canFit = inv.checkPositionsAreEmpty(droppedItem.sizeX, droppedItem.sizeY, id, droppedItem);
        if (canFit && droppedItem != null && inv.slots[droppedItem.slotId] != null)
        {
            Debug.Log("Parent " + inv.slots[droppedItem.slotId].name);
            Debug.Log("droppedItem on slot "+id);
            this.droppedItem = droppedItem;
            inv.occupyGridWithItem(droppedItem.sizeX, droppedItem.sizeY, inv.slots[droppedItem.slotId].GetComponent<Slots>().id, true, null);
            droppedItem.slotId = this.id;
            inv.occupyGridWithItem(droppedItem.sizeX, droppedItem.sizeY, id, false, droppedItem);
        }
    }
}
