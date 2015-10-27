using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    Item item;
    int quantity;
    public int sizeX;
    public int sizeY;
    public int slotId;

    private Vector2 offset;
    private Inventory inventory;
    private CanvasGroup canvasG;
    private float canvasScaleFactor;
    public Sprite sprite;

    public void Start()
    {
        canvasG = GetComponent<CanvasGroup>();
        canvasScaleFactor = inventory.GetComponentInParent<Canvas>().scaleFactor;
        sprite = GetComponent<Image>().sprite;
    }

    public void Initialize(Inventory inv, Item item, int sizeX, int sizeY) {
        this.inventory = inv;
        this.item = item;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
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
        Debug.Log(eventData.pointerEnter);
        Transform slot = inventory.slots[slotId].transform;
        if (eventData.pointerEnter != null)
        {
            Debug.Log("droppedItem on Slot");
            this.transform.SetParent(slot);
            this.transform.position = slot.transform.position;
            this.transform.localPosition = new Vector2(sprite.rect.width / 4, -sprite.rect.height / 4);
           // this.transform.SetParent(slot.parent, true);
        }
        else {
            inventory.occupyGridWithItem(sizeX, sizeY, slot.GetComponent<Slots>().id, true, null);
            Debug.Log("droppedItem out of Slot");
            Destroy(this.gameObject);
            //instantiate a pickup
        }
        canvasG.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item != null)
        {
            offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
        }
    }



}
