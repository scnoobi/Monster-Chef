using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler {

    Item item;
    int quantity;

    public void setItem(Item item) {
        this.item = item;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
            transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
