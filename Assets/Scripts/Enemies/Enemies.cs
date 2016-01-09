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
        public float MaxHP { get; set; }
        public float CurrHP { get; set; }
        public float AttackSpeed { get; set; }
        public float MovementSpeed { get; set; }
        public float Armor { get; set; }
        public float FireResist { get; set; }
        public float PoisonResist { get; set; }
        public float IceResist { get; set; }

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
        this.enemyStats = enemy.enemyStats;
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
    }

    #region events

    // Your current method for taking damage

    public override void TakeDamage(float damage)
    {
        Debug.Log("ouch " + topDownController.transform.name + " took damage " + (damage * (100 - enemyStats.Armor))/100);
        enemyStats.CurrHP -= (damage * (100 - enemyStats.Armor)) / 100;
        if (OnDamageTaken != null) OnDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    public override void TakeFireDamage(float damage)
    {
        Debug.Log("ouch " + topDownController.transform.name + " took fire damage " + (damage * (100 - enemyStats.FireResist)) / 100 + " at "+Time.time);
        enemyStats.CurrHP -= damage * (100 - enemyStats.FireResist);
        if (OnFireDamageTaken != null) OnFireDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    public override void TakePoisonDamage(float damage)
    {
        Debug.Log("ouch "+topDownController.transform.name+" took poison damage " + (damage * (100 - enemyStats.PoisonResist)) / 100 + " at " + Time.time);
        enemyStats.CurrHP -= damage * (100 - enemyStats.PoisonResist);
        if (OnPoisonDamageTaken != null) OnPoisonDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    public override void TakeIceDamage(float damage)
    {
        Debug.Log("ouch " + topDownController.transform.name + " took ice damage " + (damage * (100 - enemyStats.IceResist)) / 100 + " at " + Time.time);
        enemyStats.CurrHP -= damage * (100 - enemyStats.IceResist);
        if (OnIceDamageTaken != null) OnIceDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    #endregion

}

