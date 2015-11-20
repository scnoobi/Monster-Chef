using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CookingMenuSlot : MonoBehaviour, IDropHandler
{
    public Inventory inventory;
    public ItemDatabase itemDb;
    List<int> items = new List<int>();

    public void OnDrop(PointerEventData eventData)
    {
        ItemDraggable droppedItem = eventData.pointerDrag.GetComponent<ItemDraggable>();
        inventory.occupyGridWithItem(droppedItem.sizeX, droppedItem.sizeY, droppedItem.slotId, true, null);
        droppedItem.slotId = -1;
        droppedItem.transform.SetParent(this.transform);
        items.Add(droppedItem.getItem().id);

        if (items.Count >= 2)
        {
            ComposedFood craftedFood = itemDb.getCraftedFood(items);
            if(craftedFood != null)
                Debug.Log("Crafted "+ craftedFood.name);
        }


    }

}