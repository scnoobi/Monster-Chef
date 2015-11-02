using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MealPlanSlot : MonoBehaviour, IDropHandler
{

    public bool occupied = false;

    public void OnDrop(PointerEventData eventData)
    {
        if (!occupied)
        {
            occupied = true;
            ItemDraggable droppedItem = eventData.pointerDrag.GetComponent<ItemDraggable>();
            ((RectTransform)(droppedItem.gameObject.transform)).pivot = new Vector2(0.5f, 0.5f);
            droppedItem.transform.SetParent(this.transform);
        }
    }
}
