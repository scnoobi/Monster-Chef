using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Enemies : Actor
{
    [System.Serializable]
    public class EnemyStats
    {
        private float maxHP;
        private float currHP;
        private float attackSpeed;
        private float movSpeed;
        private float armor;
        private float fireResist;
        private float poisonResist;
        private float iceResist;
        private EnemyController enemyController;

        public EnemyStats() { }

        public EnemyStats(EnemyStats copyThis)
        {
            this.maxHP = copyThis.maxHP;
            this.currHP = copyThis.currHP;
            this.attackSpeed = copyThis.attackSpeed;
            this.armor = copyThis.armor;
            this.fireResist = copyThis.fireResist;
            this.poisonResist = copyThis.poisonResist;
            this.iceResist = copyThis.iceResist;
        }

        public void setController(EnemyController controller) {this.enemyController = controller; }

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
                if(enemyController!=null)
                    enemyController.setHealthBarValue((currHP* 100/maxHP)/100);
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
            }
        }

        public float MovementSpeed
        {
            get
            {
                return movSpeed;
            }

            set
            {
                movSpeed = value;
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

        public void FuseStats(EnemyStats statsToAdd)
        {
            MaxHP += statsToAdd.MaxHP;
            CurrHP += statsToAdd.CurrHP;
            AttackSpeed += statsToAdd.AttackSpeed;
            MovementSpeed += statsToAdd.MovementSpeed;
            Armor += statsToAdd.Armor;
            FireResist += statsToAdd.FireResist;
            IceResist += statsToAdd.IceResist;
            PoisonResist += statsToAdd.PoisonResist;
        }
    }

    public EnemyStats enemyStats;
    public List<int> charAbilitiesIndex;
    //public List<int> statusEffectsIndex;

    public Enemies()
    {
        innateAbilities = new List<Ability>();
        affectedStatusEffects = new List<StatusEffects>();
        onHitStatusEffects = new List<StatusEffects>();
    }

    public Enemies(Enemies enemy)
    {
        this.name = enemy.name;
        this.enemyStats = new EnemyStats(enemy.enemyStats);
        this.charAbilitiesIndex = new List<int>(enemy.charAbilitiesIndex);
        //this.statusEffectsIndex = new List<int>(enemy.statusEffectsIndex);
        innateAbilities = new List<Ability>();
        affectedStatusEffects = new List<StatusEffects>();
        onHitStatusEffects = new List<StatusEffects>();
    }

    public void Initialize()
    {
        AbilityDatabase abDB = GameObject.Find("Databases").GetComponent<AbilityDatabase>();
        for (int i = 0; i < charAbilitiesIndex.Count; i++)
            innateAbilities.Add(abDB.getAbilityById(charAbilitiesIndex[i]));
    }

    public void SetController(EnemyController enemyController)
    {
        this.topDownController = enemyController;
        enemyStats.setController(enemyController);
    }

    #region events

    // Your current method for taking damage

    public override void TakeDamage(float damage)
    {
        //Debug.Log("ouch " + topDownController.transform.name + " took damage " + (damage * (100 - enemyStats.Armor))/100);
        enemyStats.CurrHP -= (damage * (100 - enemyStats.Armor)) / 100;
        ((EnemyController)topDownController).FloatingDamage((damage * (100 - enemyStats.Armor)) / 100, TypeOfEffects.none);
        if (OnDamageTaken != null) OnDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    public override void TakeFireDamage(float damage)
    {
        //Debug.Log("ouch " + topDownController.transform.name + " took fire damage " + (damage * (100 - enemyStats.FireResist)) / 100 + " at "+Time.time);
        enemyStats.CurrHP -= damage * (100 - enemyStats.FireResist) / 100;
        ((EnemyController)topDownController).FloatingDamage((damage * (100 - enemyStats.FireResist)) / 100, TypeOfEffects.fire);
        if (OnFireDamageTaken != null) OnFireDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    public override void TakePoisonDamage(float damage)
    {
        Debug.Log("ouch "+topDownController.transform.name+" took poison damage " + (damage * (100 - enemyStats.PoisonResist)) / 100 + " at " + Time.time);
        enemyStats.CurrHP -= damage * (100 - enemyStats.PoisonResist) / 100;
        if (OnPoisonDamageTaken != null) OnPoisonDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    public override void TakeIceDamage(float damage)
    {
        Debug.Log("ouch " + topDownController.transform.name + " took ice damage " + (damage * (100 - enemyStats.IceResist)) / 100 + " at " + Time.time);
        enemyStats.CurrHP -= damage * (100 - enemyStats.IceResist) / 100;
        if (OnIceDamageTaken != null) OnIceDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    #endregion

}

