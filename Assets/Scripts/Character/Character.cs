using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character {

    [System.Serializable]
    public struct stats
    {
        public float maxHP;
        public float currHP;
        public float maxHunger;
        public float currHunger;
        public float attackSpeed;
        public float movementSpeed;

        public void FuseStats(stats statsToAdd)
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
    public stats characterStats;
    TasteToStats tasteTranslater;
    TopDownController controller;
    List<int> charAbilitiesIndex;
    List<Ability> charAbilities;
    List<Ability> foodAbilities;

    // Use this for initialization
    public Character() {
        charAbilities = new List<Ability>();
        foodAbilities = new List<Ability>();
        AbilityDatabase abDB = GameObject.Find("Databases").GetComponent<AbilityDatabase>();
        
        for(int i = 0; i < charAbilitiesIndex.Count; i++)
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

    public void addFoodAbilities(Ability charAbility)
    {
        foodAbilities.Add(charAbility);
    }

    public void ConsumeMeals(List<Food> mealPlan) { 
        for(int i=0; i< mealPlan.Count; i++ ){
            characterStats.FuseStats(tasteTranslater.tasteToStats(mealPlan[i]));
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
