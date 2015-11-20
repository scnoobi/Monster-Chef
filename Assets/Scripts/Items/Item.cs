using UnityEngine;
using System.Collections;

public abstract class Item {
    public int id;
    public string name;
    public enum itemType { food, others }
    public itemType typeOfItem;
}
