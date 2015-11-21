using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CookingMenuSlot : MonoBehaviour, IDropHandler
{
    public Inventory inventory;
    public ItemDatabase itemDb;
    public GameObject timer;
    public GameObject itemDraggable;
    List<ItemDraggable> dragItems = new List<ItemDraggable>();
    List<Food> items = new List<Food>();
    List<int> itemsID = new List<int>();

    public void OnDrop(PointerEventData eventData)
    {
        ItemDraggable droppedItem = eventData.pointerDrag.GetComponent<ItemDraggable>();
        inventory.occupyGridWithItem(droppedItem.sizeX, droppedItem.sizeY, droppedItem.slotId, true, null);
        ((RectTransform)(droppedItem.gameObject.transform)).pivot = new Vector2(0.5f, 0.5f);
        droppedItem.slotId = -1;
        droppedItem.transform.SetParent(this.transform);
        dragItems.Add(droppedItem);
        items.Add((Food)droppedItem.getItem());
        itemsID.Add(((Food)droppedItem.getItem()).id);

        if (items.Count >= 2)
        {
            ComposedFood craftedFood = itemDb.getCraftedFood(itemsID);
            if (craftedFood != null)
            {
                for(int i = 0; i < items.Count; i++)
                {
                    Debug.Log("Ingredients are "+items[i].name);
                    craftedFood.addFood(items[i]);
                }
                for (int i = 0; i < dragItems.Count; i++)
                {
                    GameObject.Destroy(dragItems[i].gameObject);
                }
                items = new List<Food>();
                itemsID = new List<int>();
                GameObject resultingfood = Instantiate(itemDraggable);
                resultingfood.GetComponent<ItemDraggable>().setItem(craftedFood);
                resultingfood.GetComponent<ItemDraggable>().setInv(inventory);
                resultingfood.GetComponent<ItemDraggable>().slotId = -1;
                ((RectTransform)(resultingfood.transform)).pivot = new Vector2(0.5f, 0.5f);
                resultingfood.transform.position = this.transform.position;
                resultingfood.transform.SetParent(this.transform);

                Debug.Log("Crafted " + craftedFood.name);
                Debug.Log("Crafted fat" + craftedFood.foodTaste.fat);
                Debug.Log("Crafted sweetness" + craftedFood.foodTaste.sweetness);
            }
        }


    }

    public void cookingTimer()
    {


    }

}