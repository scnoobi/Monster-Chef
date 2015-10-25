using UnityEngine;
using System.Collections;

public abstract class PickupItem : MonoBehaviour {

    public int sizeX;
    public int sizeY;
    public Sprite inventorySprite;

    public abstract Item createItem();
}
