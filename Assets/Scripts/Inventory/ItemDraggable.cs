using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    Item item;
    int quantity;
    public int sizeX;
    public int sizeY;

    public Transform originalParent;
    private Vector2 offset;
    private Inventory inventory;
    private CanvasGroup canvasG;

    public void Start()
    {
        inventory = GetComponentInParent<Inventory>();
        canvasG = GetComponent<CanvasGroup>();
    }

    public void setItem(Item item) {
        this.item = item;
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
            originalParent = this.transform.parent;
            this.transform.SetParent(this.transform.parent.parent);
            transform.position = eventData.position - offset;
            canvasG.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
            transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(originalParent);
        this.transform.position = originalParent.transform.position;
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
