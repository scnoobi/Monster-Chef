using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class CookingMenuSlot : MonoBehaviour, IDropHandler
{
    public Inventory inventory;
    public ItemDatabase itemDb;
    public GameObject timer;
    public GameObject itemDraggable;
    List<ItemDraggable> dragItems = new List<ItemDraggable>();
    List<Food> items = new List<Food>();
    List<int> itemsID = new List<int>();
    private bool cooking = false;
    public const float MEDIUM = 1f/2f;
    public const float WELL_DONE = 3f/4f;
    public const float BURNT = 9f/10f;

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
        startCooking();
    }

    public void clean()
    {
        items = new List<Food>();
        itemsID = new List<int>();
        dragItems = new List<ItemDraggable>();
    }

    public void startCooking()
    {
        if(!cooking)
            cooking = true;
        else
        {
            timer.GetComponent<Image>().fillAmount -= 0.4f;
        }
    }

    public ComposedFood cook()
    {
        ComposedFood craftedFood = null;
        craftedFood = itemDb.getCraftedFood(itemsID);
        Debug.Log(craftedFood);
        if (craftedFood != null)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Debug.Log("Ingredients are " + items[i].name);
                craftedFood.addFood(items[i]);
            }
            for (int i = 0; i < dragItems.Count; i++)
            {
                Destroy(dragItems[i].gameObject);
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

            dragItems.Add(resultingfood.GetComponent<ItemDraggable>());
            items.Add(craftedFood);
            itemsID.Add(craftedFood.id);
        }
        else
        {
            ((Food)dragItems[dragItems.Count - 1].getItem()).foodTaste.complexTaste(((Food)dragItems[dragItems.Count-1].getItem()).foodTaste);
            Destroy(dragItems[dragItems.Count - 1].gameObject);
        }
        return craftedFood;
    }

    public void FinishCooking(ComposedFood food)
    {
        float time = timer.GetComponent<Image>().fillAmount;

        if (time < MEDIUM) { //mal passado
            Debug.Log("rare");
            food.weakenTaste();
        }else if(time < WELL_DONE) { //ponto
            Debug.Log("medium");
        }
        else if(time < BURNT) //bem passado
        {
            Debug.Log("well done");
            food.strenghtenTaste();
        }
        else //queimado
        {
            Debug.Log("burned");
        }
        //throw new NotImplementedException();
    }

    public void changeColorOfTimer()
    {
        Image timerImage = timer.GetComponent<Image>();
        float time = timerImage.fillAmount;
        if (time < MEDIUM)
        { 
            timerImage.color = Color.green;
        }
        else if (time < WELL_DONE)
        { 
            timerImage.color = Color.yellow;
        }
        else if (time < BURNT) 
        {
            timerImage.color = Color.red;
        }
        else
        {
            timerImage.color = Color.black;
        }

    }

    public void stopCooking()
    {
        cooking = false;
    }

    public void Update()
    {
        if (cooking)
        {
            timer.GetComponent<Image>().fillAmount += .1f * Time.deltaTime;
            changeColorOfTimer();
            if (timer.GetComponent<Image>().fillAmount == 1)
            {
                ComposedFood result = cook();
                FinishCooking(result);
                cooking = false;
                timer.GetComponent<Image>().fillAmount = 0;
            }
        }
        else if (timer.GetComponent<Image>().fillAmount > 0)
        {
            ComposedFood result = cook();
            FinishCooking(result);
            cooking = false;
            timer.GetComponent<Image>().fillAmount = 0;
        }
    }
}