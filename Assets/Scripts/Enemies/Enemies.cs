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
    EnemyController enemyController;

    public Enemies()
    {
        innateAbilities = new List<Ability>();
    }

    public void Initialize()
    {
        AbilityDatabase abDB = GameObject.Find("Databases").GetComponent<AbilityDatabase>();

        for (int i = 0; i < charAbilitiesIndex.Count; i++)
            innateAbilities.Add(abDB.getAbilityById(charAbilitiesIndex[i]));
    }

    public void SetController(EnemyController enemyController)
    {
        this.enemyController = enemyController;
    }

    #region events

    // Your current method for taking damage

    public override void TakeDamage(float damage)
    {
        enemyStats.CurrHP -= damage * (100 - enemyStats.Armor);
        if (OnDamageTaken != null) OnDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    public override void TakeFireDamage(float damage)
    {
        enemyStats.CurrHP -= damage * (100 - enemyStats.FireResist);
        if (OnFireDamageTaken != null) OnFireDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    public override void TakePoisonDamage(float damage)
    {
        enemyStats.CurrHP -= damage * (100 - enemyStats.PoisonResist);
        if (OnPoisonDamageTaken != null) OnPoisonDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    public override void TakeIceDamage(float damage)
    {
        enemyStats.CurrHP -= damage * (100 - enemyStats.IceResist);
        if (OnIceDamageTaken != null) OnIceDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }

    #endregion

}

