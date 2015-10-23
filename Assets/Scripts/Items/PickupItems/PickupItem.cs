using UnityEngine;
using System.Collections;

public abstract class PickupItem : MonoBehaviour {

    public int sizeX;
    public int sizeY;

    public abstract Item createItem();
}
