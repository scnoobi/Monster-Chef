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

   public void Initialize(string name, Food.taste foodTaste, float timeToCook, Food.cookingType currentCookingType)
   {
       this.name = name;
       this.foodTaste = foodTaste;
       this.timeToCook = timeToCook;
       this.currentCookingType = currentCookingType;
   }
}
