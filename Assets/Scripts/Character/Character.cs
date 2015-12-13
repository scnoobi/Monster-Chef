using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Character : Actor {

    [System.Serializable]
    public class Stats
    {
        private float maxHP;
        private float currHP;
        private float maxHunger;
        private float currHunger;
        private float attackSpeed;
        private float movementSpeed;

        public float MaxHP { get { return maxHP; } set { maxHP = value; } }
        public float CurrHP { get { return currHP; } set { currHP = value; } }
        public float MaxHunger { get { return maxHunger; } set { maxHunger = value; } }
        public float CurrHunger { get { return currHunger; } set { currHunger = value; } }
        public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
        public float MovementSpeed { get { return movementSpeed; } set { movementSpeed = value; } }

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

    public Stats characterStats;
    public TasteToStats tasteTranslater;
    public List<int> charAbilitiesIndex;

    TopDownController controller;
    List<Ability> foodAbilities;
    AbilityDatabase abDB;
    SkillMenu skillMenu;

    // Use this for initialization
    public Character()
    {
        innateAbilities = new List<Ability>();
        foodAbilities = new List<Ability>();
    }

    public void Initialize()
    {
        skillMenu = GameObject.Find("Food Skills").GetComponent<SkillMenu>();
        skillMenu.SetCharacter(this);
        abDB = GameObject.Find("Databases").GetComponent<AbilityDatabase>();
        SkillMenu characterSkills = GameObject.Find("Character Skills").GetComponent<SkillMenu>();
        characterSkills.SetCharacter(this);

        for (int i = 0; i < charAbilitiesIndex.Count; i++)
            addCharAbilities(abDB.getAbilityById(charAbilitiesIndex[i]));

        tasteTranslater = new SimpleTasteTranslation();
        skillMenu.updateSkillList();
        characterSkills.updateSkillList();
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
        innateAbilities.Add(charAbility);
        charAbility.setCaster(this);
    }

    public void addFoodAbilities(Ability foodAbility)
    {
        foodAbilities.Add(foodAbility);
        foodAbility.setCaster(this);
    }

    public List<Ability> getCharAbilities()
    {
        return innateAbilities;
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
        if(index < innateAbilities.Count)
        {
            Debug.Log("Character Ability");
            castAbility = innateAbilities[index];
        }
        else if(index < innateAbilities.Count + foodAbilities.Count)
        {
            Debug.Log("Food Ability");
            castAbility = foodAbilities[index- innateAbilities.Count];
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
