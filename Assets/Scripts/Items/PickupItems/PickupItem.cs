using UnityEngine;
using System.Collections;

public abstract class PickupItem : MonoBehaviour {

    protected int sizeX;
    protected int sizeY;
    public Sprite inventorySprite;

    public int getSizeX()
    {
        return sizeX;
    }

    public int getSizeY()
    {
        return sizeY;
    }

    public void setSizeX(int sizeX)
    {
        this.sizeX = sizeX;
    }

    public void setSizeY(int sizeY)
    {
        this.sizeY = sizeY;
    }

    public abstract Item getItem();
}
