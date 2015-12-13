using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class ReflectDamage : Ability
{

    public int damage;
    Character myCharacter;

    void OnDamageTaken(object sender, EventArgs e)
    {
        castAbility();
    }

    public override void castAbility()
    {
        Debug.Log("Reflect damage");
    }

    public override void setCaster(Character caster)
    {
        myCharacter = caster;
        myCharacter.OnDamageTaken += OnDamageTaken;
    }

    public override void weakenedAbility()
    {
        damage = Mathf.Clamp(damage - 2, 0, int.MaxValue);
    }

    public override void enhancedAbility()
    {
        //throw new NotImplementedException();
    }
}

