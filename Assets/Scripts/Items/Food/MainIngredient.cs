using UnityEngine;
using System.Collections;

public class MainIngredient : Ingredient{

    public MainIngredient()
    {
        this.name = "";
        this.timeToCook = 0f;
        this.typeOfItem = itemType.food;
    }

    public MainIngredient(string name, taste foodTaste, float timeToCook, cookingType currentCookingType) {
        this.name = name;
        this.foodTaste = foodTaste;
        this.timeToCook = timeToCook;
        this.currentCookingMethod = currentCookingMethod;
        this.typeOfItem = itemType.food;
    }
}
