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
    private int Itemindex;

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

    public void Initialize(Inventory inv, int Itemindex, Item item, int sizeX, int sizeY, Sprite groundSprite)
    {
        this.inventory = inv;
        this.Itemindex = Itemindex;
        this.item = item;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.groundSprite = groundSprite;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            this.transform.SetParent(this.transform.parent.parent);
            transform.position = eventData.position - offset;
            canvasG.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
            transform.position = eventData.position - offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Transform slot = inventory.slots[slotId].transform;
        if (eventData.pointerEnter != null)
        {
            GameObject Nslot = inventory.MapPositionToSlot(transform.position);
            if (Nslot != null)
            {
                Slots slotScript = Nslot.GetComponent<Slots>();
                bool canFit = inventory.checkPositionsAreEmpty(sizeX, sizeY, slotScript.id, this);
                if (canFit)
                {
                    inventory.occupyGridWithItem(sizeX, sizeY, slotId, true, null);
                    this.slotId = slotScript.id;
                    inventory.occupyGridWithItem(sizeX, sizeY, slotId, false, this);
                    slotScript.itemID = this.GetInstanceID();
                    this.transform.SetParent(Nslot.transform);
                }else
                    this.transform.SetParent(slot);
            }
            else      
                this.transform.SetParent(slot);

            this.transform.localPosition = new Vector2(0, 0);
        }
        else {
            inventory.occupyGridWithItem(sizeX, sizeY, slot.GetComponent<Slots>().id, true, null);
            createPickup(Camera.main.ScreenToWorldPoint(eventData.position));
            Destroy(this.gameObject);
        }
        canvasG.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item != null)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
                offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
            else if (eventData.button == PointerEventData.InputButton.Right) {
                if (item.typeOfItem == Item.itemType.food)
                {
                    inventory.consumeFood(Itemindex);
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
