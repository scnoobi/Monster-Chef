using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MealPlanSlot : MonoBehaviour, IDropHandler
{
    public ItemDraggable currentItem;
    public Inventory inventory;

    public bool occupied = false;

    public void OnDrop(PointerEventData eventData)
    {
        if (!occupied)
        {
            occupied = true;
            ItemDraggable droppedItem = eventData.pointerDrag.GetComponent<ItemDraggable>();
            inventory.occupyGridWithItem(droppedItem.sizeX, droppedItem.sizeY, droppedItem.slotId, true, null);
            droppedItem.slotId = -1;
            currentItem = droppedItem;
            ((RectTransform)(droppedItem.gameObject.transform)).pivot = new Vector2(0.5f, 0.5f);
            droppedItem.transform.SetParent(this.transform);
        }
    }
}
