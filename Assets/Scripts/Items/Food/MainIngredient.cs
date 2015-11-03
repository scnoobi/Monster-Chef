using UnityEngine;
using System.Collections;

public class MainIngredient : Ingredient{

    public MainIngredient(string name, taste foodTaste, float timeToCook, cookingType currentCookingType) {
        this.name = name;
        this.foodTaste = foodTaste;
        this.timeToCook = timeToCook;
        this.currentCookingMethod = currentCookingMethod;
        this.typeOfItem = itemType.food;
    }
}
