using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {

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

    public stats characterStats;
    TasteToStats tasteTranslater;
    TopDownController controller;
    List<Ability> charAbilities;
    List<Ability> foodAbilities;

    // Use this for initialization
    void Start () {
        charAbilities = new List<Ability>();
        foodAbilities = new List<Ability>();

        addCharAbilities(new ShootAbility(1, "Ice_Ball")); //TODO: change character costumization to a json file

        tasteTranslater = new SimpleTasteTranslation();
        controller = GetComponent<TopDownController>();
        controller.setMaxSpeed(characterStats.movementSpeed);
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
