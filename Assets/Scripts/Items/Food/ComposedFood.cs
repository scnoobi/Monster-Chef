using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComposedFood : Food
{
    List<Food> foodParts;
    recipe recipeOfFood;

    [System.Serializable]
    public struct recipe
    {
        int[] input;
        int output;
    }

 public ComposedFood(Food food1, Food food2)
    {
        foodParts.Add(food1);
        foodParts.Add(food2);
    }

    public ComposedFood()
    {
    }

    public void addFood(Food part){
        foodParts.Add(part);
        foodTaste.complexTaste(part.foodTaste);
    }
}
