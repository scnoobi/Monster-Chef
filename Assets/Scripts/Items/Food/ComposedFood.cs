using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComposedFood : Food
{
    List<Food> foodParts;
    public int foodAbilityIndex;
    Ability foodAbility;
    public const float WEAKNED_TASTE = .75f;
    public const float STRENGHNED_TASTE = 1.25f;

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

    public ComposedFood()
    {
        AbilityDatabase abDB = GameObject.Find("Databases").GetComponent<AbilityDatabase>();
        foodParts = new List<Food>();
        foodAbility = abDB.getAbilityById(foodAbilityIndex);
    }

    public int getAbilityIndex()
    {
        return foodAbilityIndex;
    }

    public Ability getAbility()
    {
        return foodAbility;
    }

    public void addFood(Food part){
        foodParts.Add(part);
        foodTaste.complexTaste(part.foodTaste);
    }

    public void weakenTaste()
    {
        foodTaste.complexTaste(WEAKNED_TASTE);
    }

    public void strenghtenTaste()
    {
        foodTaste.complexTaste(STRENGHNED_TASTE);
    }

    public void weakenAbility()
    {
        foodAbility.weakenedAbility();
    }

    public void strenghtenAbility()
    {
        foodAbility.enhancedAbility();
    }
}
