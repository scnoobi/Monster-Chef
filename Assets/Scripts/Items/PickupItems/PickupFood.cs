using UnityEngine;
using System.Collections;

public class PickupFood : PickupItem {

    public string name;
    private ItemDatabase itemDB;
    private Food food;
    public enum typesOfFood { meat, fish, veggies } 
    
    override public Item getItem()
    {
        return getFood();
    }

    public void Start() {
        itemDB = GameObject.Find("itemDB").GetComponent<ItemDatabase>();
        food = (Food)itemDB.getItemByName(name);
    }

    public Food getFood()
    {
        return food;
    }

    public void Initialize(string name, Food.taste foodTaste, float timeToCook, Food.cookingType currentCookingType)
   {
       this.name = name;
       food = (Food)itemDB.getItemByName(name);
   }
}
