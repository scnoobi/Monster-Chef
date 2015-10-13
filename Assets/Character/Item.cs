using UnityEngine;
using System.Collections;

public class Item {
    public int height;
    public int weight;
    public string itemLabel;

    public Item(int h, int w) { 
        height = h;
        weight = w;
    }

    public Item(int h, int w, string itemLabel)
    {
        height = h;
        weight = w;
        this.itemLabel = itemLabel;
    }

}
