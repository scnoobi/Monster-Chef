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
        if (canFit && droppedItem!=null)
        {
            this.droppedItem = droppedItem;
            inv.occupyGridWithItem(droppedItem.sizeX, droppedItem.sizeY, droppedItem.originalParent.GetComponent<Slots>().id, true, null);
            droppedItem.originalParent = this.transform;
            inv.occupyGridWithItem(droppedItem.sizeX, droppedItem.sizeY, id, false, droppedItem);
        }
    }
}
