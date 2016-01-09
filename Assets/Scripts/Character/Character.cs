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
        private float maxStamina;
        private float currStamina;
        private float armor;
        private float attackDamage;
        private float attackSpeed;
        private float movementSpeed;
        private float fireResist;
        private float iceResist;
        private float poisonResist;
        private Character character;

        public float MaxHunger { get; set; }
        public float CurrHunger { get; set; }

        public float MaxHP
        {
            get
            {
                return maxHP;
            }

            set
            {
                maxHP = value;
            }
        }
        public float CurrHP
        {
            get
            {
                return currHP;
            }

            set
            {
                currHP = value;
            }
        }
        public float MaxStamina
        {
            get
            {
                return maxStamina;
            }

            set
            {
                maxStamina = value;
            }
        }
        public float CurrStamina
        {
            get
            {
                return currStamina;
            }

            set
            {
                currStamina = value;
            }
        }
        public float Armor
        {
            get
            {
                return armor;
            }

            set
            {
                armor = value;
            }
        }
        public float AttackDamage
        {
            get
            {
                return attackDamage;
            }

            set
            {
                attackDamage = value;
            }
        }
        public float AttackSpeed
        {
            get
            {
                return attackSpeed;
            }

            set
            {
                attackSpeed = value;
                if(character != null)
                    character.topDownController.animator.SetFloat("attackSpeed", value);
            }
        }
        public float MovementSpeed
        {
            get
            {
                return movementSpeed;
            }

            set
            {
                movementSpeed = value;
                if (character != null)
                {
                    character.topDownController.maxSpeed = value;
                    character.topDownController.animator.SetFloat("movementSpeed", value);
                }
            }
        }
        public float FireResist
        {
            get
            {
                return fireResist;
            }

            set
            {
                fireResist = value;
            }
        }
        public float IceResist
        {
            get
            {
                return iceResist;
            }

            set
            {
                iceResist = value;
            }
        }
        public float PoisonResist
        {
            get
            {
                return poisonResist;
            }

            set
            {
                poisonResist = value;
            }
        }
    
        public void setChar(Character character) { this.character = character; }
        public void FuseStats(Stats statsToAdd)
        {
            MaxHP += statsToAdd.MaxHP;
            CurrHP += statsToAdd.CurrHP;
            MaxStamina += statsToAdd.MaxStamina;
            CurrStamina += statsToAdd.CurrStamina;
            Armor += statsToAdd.Armor;
            AttackDamage += statsToAdd.AttackDamage;
            AttackSpeed += statsToAdd.AttackSpeed;
            MovementSpeed += statsToAdd.MovementSpeed;
            FireResist += statsToAdd.FireResist;
            IceResist += statsToAdd.IceResist;
            PoisonResist += statsToAdd.PoisonResist;
        }
    }

    public Stats characterStats;
    public TasteToStats tasteTranslater;
    public List<int> charAbilitiesIndex;
    //public List<int> statusEffectsIndex;

    List<Ability> foodAbilities;
    AbilityDatabase abDB;
    SkillMenu skillMenu;
    SimpleAttack attack;
    public bool attacksMelee = true;

    // Use this for initialization
    public Character()
    {
        innateAbilities = new List<Ability>();
        foodAbilities = new List<Ability>();
        affectedStatusEffects = new List<StatusEffects>();
        onHitStatusEffects = new List<StatusEffects>();
    }

    public void Initialize(MyCharacterController controller)
    {
        characterStats.setChar(this);
        this.topDownController = controller;

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
        //to refresh their setter
        characterStats.MovementSpeed = characterStats.MovementSpeed;
        characterStats.AttackSpeed = characterStats.AttackSpeed;

        DamageOverTime dot = new DamageOverTime();
        dot.duration = 3;
        dot.damage = 1;
        dot.hitsPerSecond = 1;
        dot.ChanceOfApplying = 25;
        dot.typeOfDamage = TypeOfEffects.fire;
        onHitStatusEffects.Add(dot);
    }

    public Character(Character character)
    {
        this.name = character.name;
        this.tasteTranslater = character.tasteTranslater;
        this.characterStats = character.characterStats;
        this.charAbilitiesIndex = new List<int>(character.charAbilitiesIndex);
        //this.statusEffectsIndex = new List<int>(character.statusEffectsIndex);
        innateAbilities = new List<Ability>();
        foodAbilities = new List<Ability>();
        affectedStatusEffects = new List<StatusEffects>();
        onHitStatusEffects = new List<StatusEffects>();
    }

    public MyCharacterController getController()
    {
        return (MyCharacterController)topDownController;
    }

    public Transform getTransform()
    {
        return topDownController.transform;
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

    public void Attack()
    {
        //attack.Attack(characterStats.AttackDamage);
    }

    #region events
    // Your current method for taking damage
    public override void TakeDamage(float damage)
    {
        characterStats.CurrHP -= damage*(100-characterStats.Armor);
        if (OnDamageTaken != null) OnDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    public override void TakeFireDamage(float damage)
    {
        characterStats.CurrHP -= damage * (100 - characterStats.FireResist);
        if (OnFireDamageTaken != null) OnFireDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    public override void TakePoisonDamage(float damage)
    {
        characterStats.CurrHP -= damage * (100 - characterStats.PoisonResist);
        if (OnPoisonDamageTaken != null) OnPoisonDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    public override void TakeIceDamage(float damage)
    {
        characterStats.CurrHP -= damage * (100 - characterStats.IceResist);
        if (OnIceDamageTaken != null) OnIceDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    #endregion
}
