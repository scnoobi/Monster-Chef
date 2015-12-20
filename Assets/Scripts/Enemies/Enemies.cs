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

        public void FuseStats(EnemyStats statsToAdd)
        {
            MaxHP += statsToAdd.MaxHP;
            CurrHP += statsToAdd.CurrHP;
            AttackSpeed += statsToAdd.AttackSpeed;
            MovementSpeed += statsToAdd.MovementSpeed;
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
        enemyStats.CurrHP -= damage;
        if (OnDamageTaken != null) OnDamageTaken(this, EventArgs.Empty);// basically, call this every time you want this event to fire (for all abilities)
    }
    #endregion

}

