using UnityEngine;
using System.Collections;

public abstract class Item {
    public int id;
    public string name;
    public string realName;
    public int sizeX;
    public int sizeY;
    public enum itemType { food, others }
    public itemType typeOfItem;
}
