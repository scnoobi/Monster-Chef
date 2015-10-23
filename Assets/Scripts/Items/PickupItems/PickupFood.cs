using UnityEngine;
using System.Collections;

public class PickupFood : PickupItem {

    public Food.cookingType currentCookingType;
    public float timeToCook;
    public string name;
    public Food.taste foodTaste;
    public typesOfFood typeOfFood;

    public enum typesOfFood { meat, fish, veggies } 
    
   override
    public Item createItem()
    {
        return new MainIngredient( name, foodTaste, timeToCook, currentCookingType);
    }
}
