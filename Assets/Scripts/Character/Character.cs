﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character {

    [System.Serializable]
    public struct Stats
    {
        public float maxHP;
        public float currHP;
        public float maxHunger;
        public float currHunger;
        public float attackSpeed;
        public float movementSpeed;

        public void FuseStats(Stats statsToAdd)
        {
            this.maxHP += statsToAdd.maxHP;
            this.currHP += statsToAdd.currHP;
            this.maxHunger += statsToAdd.maxHunger;
            this.currHunger += statsToAdd.currHunger;
            this.attackSpeed += statsToAdd.attackSpeed;
            this.movementSpeed += statsToAdd.movementSpeed;
        }
    }

    public string name;
    public Stats characterStats;
    public TasteToStats tasteTranslater;
    public List<int> charAbilitiesIndex;

    TopDownController controller;
    List<Ability> charAbilities;
    List<Ability> foodAbilities;
    AbilityDatabase abDB;

    // Use this for initialization
    public Character() {
        charAbilities = new List<Ability>();
        foodAbilities = new List<Ability>();
	}
	
    public void Initialize()
    {
        abDB = GameObject.Find("Databases").GetComponent<AbilityDatabase>();

        for (int i = 0; i < charAbilitiesIndex.Count; i++)
            addCharAbilities(abDB.getAbilityById(charAbilitiesIndex[i]));

        tasteTranslater = new SimpleTasteTranslation();
    }


    public void setController(TopDownController controller)
    {
        this.controller = controller;
        this.controller.setMaxSpeed(characterStats.movementSpeed);
    }

    public void addCharAbilities(Ability charAbility)
    {
        charAbilities.Add(charAbility);
    }

    public void addFoodAbilities(Ability foodAbility)
    {
        foodAbilities.Add(foodAbility);
    }

    public void ConsumeMeals(List<Food> mealPlan) { 
        for(int i=0; i< mealPlan.Count; i++ ){
            characterStats.FuseStats(tasteTranslater.tasteToStats(mealPlan[i]));
            if(mealPlan.GetType() == typeof(ComposedFood))
                addFoodAbilities(abDB.getAbilityById(((ComposedFood)mealPlan[i]).getAbility()));
        }
    }

    public void castCorrectAbility(int index)
    {
        Ability castAbility = null;
        if(index < charAbilities.Count)
        {
            castAbility = charAbilities[index];
        }
        else if(index < foodAbilities.Count)
        {
            castAbility = foodAbilities[index];
        }
        if(castAbility != null)
            castAbility.castAbility();
    }

}
