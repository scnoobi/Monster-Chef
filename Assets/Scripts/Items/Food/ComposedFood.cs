using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComposedFood : Food
{
    List<Food> foodParts;
    Ability foodAbility;

    [System.Serializable]
    public struct recipe
    {
        public List<int> input;
        public int output;

        public recipe (List<int> input, int output)
        {
            this.input = input;
            this.output = output;
        }
    }

    public ComposedFood(Food food1, Food food2)
    {
        foodParts.Add(food1);
        foodParts.Add(food2);
    }

    public ComposedFood()
    {
        foodParts = new List<Food>();
    }

    public void setAbility(string name)
    {

    }

    public void addFood(Food part){
        foodParts.Add(part);
        foodTaste.complexTaste(part.foodTaste);
    }
}
