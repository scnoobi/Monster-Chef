using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    Item item;
    int quantity;
    public int sizeX;
    public int sizeY;
    public int slotId;
    public GameObject blankPickUp;

    private Vector2 offset;
    private Vector2 offsetInSlots;
    private Inventory inventory;
    private CanvasGroup canvasG;
    private float canvasScaleFactor;
    private Sprite groundSprite;
    
    public Sprite sprite;

    public void Start()
    {
        canvasG = GetComponent<CanvasGroup>();
        canvasScaleFactor = inventory.GetComponentInParent<Canvas>().scaleFactor;
        sprite = GetComponent<Image>().sprite;
    }

    public void Initialize(Inventory inv, Item item, int sizeX, int sizeY, Sprite groundSprite)
    {
        this.inventory = inv;
        this.item = item;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.groundSprite = groundSprite;
    }

    public void setItem(Item item)
    {
        this.item = item;
    }


    public void setInv(Inventory inv)
    {
        this.inventory = inv;
    }

    public void setSize(int sizeX, int sizeY)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            if (transform.parent.tag.Equals("MealPlan"))
            {
                transform.parent.GetComponent<MealPlanSlot>().occupied = false;
                transform.parent.GetComponent<MealPlanSlot>().currentItem = null;
            }
            if (transform.parent.tag.Equals("CookingMenu")) { 
                
            }
            if (transform.parent.tag.Equals("Inventory"))
                this.transform.SetParent(this.transform.parent.parent);
            transform.position = eventData.position - offset;
            canvasG.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            transform.position = eventData.position - offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Transform slot = null;
        if(slotId >= 0)
            slot = inventory.slots[slotId].transform;

        if (eventData.pointerEnter != null) // if is dropped inside an interface
        {
            Vector3 position = transform.position;
            if (transform.GetComponent<RectTransform>().pivot.x == .5f)
            {
                position = new Vector3(position.x - GetComponent<RectTransform>().rect.width, position.y + GetComponent<RectTransform>().rect.width, position.z);
            }
            GameObject Nslot = inventory.MapPositionToSlot(position);
            if (Nslot != null) //if is dropped inside the inventory slot grid
            {
                Slots slotScript = Nslot.GetComponent<Slots>();
                bool canFit = inventory.checkPositionsAreEmpty(sizeX, sizeY, slotScript.id, this);
                if (canFit) //if can fit
                {
                    inventory.occupyGridWithItem(sizeX, sizeY, slotId, true, null);
                    this.slotId = slotScript.id;
                    inventory.occupyGridWithItem(sizeX, sizeY, slotId, false, this);
                    slotScript.itemID = this.GetInstanceID();
                    this.transform.SetParent(Nslot.transform);
                }
                else if (slotId >= 0)// if is dropped inside the inventory slot grid AND it cant fit AND its previous parent was an inventory slot grid
                {
                    this.transform.SetParent(slot);
                }
            }
            else if (slotId >= 0)// if is dropped outside the inventory slot grid AND its previous parent was an inventory slot grid
            {
                this.transform.SetParent(slot);
            }

            this.transform.localPosition = new Vector2(0, 0);

            if (transform.parent.tag.Equals("Inventory"))
                ((RectTransform)(gameObject.transform)).pivot = new Vector2(0f, 1f);
        }
        else //if is dropped outside an interface
        {
            Debug.Log("if is dropped outside an interface");
            inventory.occupyGridWithItem(sizeX, sizeY, slotId, true, null);
            createPickup(Camera.main.ScreenToWorldPoint(eventData.position));
            Destroy(this.gameObject);
        }

        canvasG.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item != null)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (item.typeOfItem == Item.itemType.food)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (item != null)
        {

        }
    }

    public void createPickup(Vector3 pos) {
        pos = new Vector3(pos.x, pos.y, 0);
        GameObject pickUpToInstantiate = (GameObject)Instantiate(blankPickUp, pos, blankPickUp.transform.rotation);
        if (item.typeOfItem == Item.itemType.food)
        {
           PickupFood foodComp = pickUpToInstantiate.AddComponent<PickupFood>();
           foodComp.sizeX = sizeX;
           foodComp.sizeY = sizeY;
           foodComp.inventorySprite = this.sprite;
           Food realFood = (Food)item;
           foodComp.Initialize(realFood.name, realFood.foodTaste, realFood.timeToCook, realFood.currentCookingMethod);
           pickUpToInstantiate.GetComponent<SpriteRenderer>().sprite = groundSprite;
        }
    }

    public Item getItem()
    {
        return item;
    }
}
