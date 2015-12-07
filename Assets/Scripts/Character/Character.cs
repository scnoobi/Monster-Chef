using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Character {

    [System.Serializable]
    public class Stats
    {
        public float MaxHP { get; set; }
        public float CurrHP { get; set; }
        public float MaxHunger { get; set; }
        public float CurrHunger { get; set; }
        public float AttackSpeed { get; set; }
        public float MovementSpeed { get; set; }

        public void FuseStats(Stats statsToAdd)
        {
            MaxHP += statsToAdd.MaxHP;
            CurrHP += statsToAdd.CurrHP;
            MaxHunger += statsToAdd.MaxHunger;
            CurrHunger += statsToAdd.CurrHunger;
            AttackSpeed += statsToAdd.AttackSpeed;
            MovementSpeed += statsToAdd.MovementSpeed;
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
    SkillMenu skillMenu;

    public EventHandler OnDamageTaken;

    // Use this for initialization
    public Character() {
        charAbilities = new List<Ability>();
        foodAbilities = new List<Ability>();
        skillMenu = GameObject.Find("Food Skills").GetComponent<SkillMenu>();
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
        this.controller.setMaxSpeed(characterStats.MovementSpeed);
    }

    public Transform getTransform()
    {
        return controller.transform;
    }

    public void addCharAbilities(Ability charAbility)
    {
        charAbilities.Add(charAbility);
        charAbility.setCaster(this);
    }

    public void addFoodAbilities(Ability foodAbility)
    {
        foodAbilities.Add(foodAbility);
        foodAbility.setCaster(this);
    }

    public List<Ability> getFoodAbilities()
    {
        return foodAbilities;
    }


    public void ConsumeMeals(List<Food> mealPlan) {
        for (int i=0; i< mealPlan.Count; i++ ){
            characterStats.FuseStats(tasteTranslater.tasteToStats(mealPlan[i]));
            if (mealPlan[i].GetType() == typeof(ComposedFood))
            {
                addFoodAbilities(((ComposedFood)mealPlan[i]).getAbility());
                skillMenu.updateSkillList();
            }
        }
    }

    public void castCorrectAbility(int index)
    {
        Ability castAbility = null;
        if(index < charAbilities.Count)
        {
            Debug.Log("Character Ability");
            castAbility = charAbilities[index];
        }
        else if(index < charAbilities.Count + foodAbilities.Count)
        {
            Debug.Log("Food Ability");
            castAbility = foodAbilities[index- charAbilities.Count];
        }
        if(castAbility != null && castAbility.isActiveAbility)
            castAbility.castAbility();
    }

    #region events

    // Your current method for taking damage
    public void TakeDamage(float damage)
    {
        characterStats.CurrHP -= damage;
        if (OnDamageTaken != null) OnDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }
    #endregion
}
