using UnityEngine;
using System.Collections;

public class PickupFood : PickupItem {

    private Food.cookingType currentCookingType;
    private float timeToCook;
    public string name;
    private Food.taste foodTaste;
    private typesOfFood typeOfFood;

    private bool settedItem = false;

    public enum typesOfFood { meat, fish, veggies } 
    
    override public Item createItem()
    {
        return new MainIngredient( name, foodTaste, timeToCook, currentCookingType);
    }

    public void Start() {
        if (!settedItem)
        {
            Debug.Log("NotEmpty");
        }
    }

    public void Initialize(string name, Food.taste foodTaste, float timeToCook, Food.cookingType currentCookingType)
   {
       settedItem = true;
       this.name = name;
       this.foodTaste = foodTaste;
       this.timeToCook = timeToCook;
       this.currentCookingType = currentCookingType;
   }
}
