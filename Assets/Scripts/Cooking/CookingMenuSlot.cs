using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CookingMenuSlot : MonoBehaviour, IDropHandler
{
    public Inventory inventory;
    public void OnDrop(PointerEventData eventData)
    {
        ItemDraggable droppedItem = eventData.pointerDrag.GetComponent<ItemDraggable>();
        inventory.occupyGridWithItem(droppedItem.sizeX, droppedItem.sizeY, droppedItem.slotId, true, null);
        droppedItem.slotId = -1;
        droppedItem.transform.SetParent(this.transform);


    }

}