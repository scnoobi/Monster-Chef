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

	// Use this for initialization
	void Start () {
        tasteTranslater = new SimpleTasteTranslation();
        controller = GetComponent<TopDownController>();
        controller.setMaxSpeed(characterStats.movementSpeed);
	}
	
   public void ConsumeMeals(List<Food> mealPlan) { 
        for(int i=0; i< mealPlan.Count; i++ ){
            characterStats.FuseStats(tasteTranslater.tasteToStats(mealPlan[i]));
        }
    }

}
