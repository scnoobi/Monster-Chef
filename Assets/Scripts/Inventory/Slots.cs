using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Slots : MonoBehaviour, IDropHandler {

    private Inventory inv;
    public int id;

	void Start () {
        inv = GetComponentInParent<Inventory>();
	}

    public void OnDrop(PointerEventData eventData)
    {
        ItemDraggable droppedItem = eventData.pointerDrag.GetComponent<ItemDraggable>();
        if (inv.checkPositionsAreEmpty(droppedItem.sizeX, droppedItem.sizeY, id)) {
            Debug.Log(droppedItem.sizeX + " " + droppedItem.sizeY + " " + id);
            inv.occupyGridWithItem(droppedItem.sizeX, droppedItem.sizeY, droppedItem.originalParent.GetComponent<Slots>().id, true);
            droppedItem.originalParent = this.transform;
            inv.occupyGridWithItem(droppedItem.sizeX, droppedItem.sizeY, id, false);
        }
    }
}
