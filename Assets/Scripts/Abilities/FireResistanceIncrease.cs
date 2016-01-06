using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class FireResistanceIncrease : Ability
{
    int changeByValue;
    float enhanceHealingValue;
    Character myCharacter;

    void OnDamageTaken(object sender, EventArgs e)
    {
        extraEffect();
    }

    public override void castAbility()
    {
        myCharacter.characterStats.FireResist += changeByValue;
    }

    public void extraEffect()
    {
        myCharacter.characterStats.CurrHP += enhanceHealingValue;     //change to a better value
    }

    public override void enhancedAbility() //also heals
    {
        enhanced = true;
    }

    public override void setCaster(Character caster) //
    {
        myCharacter = caster;
        castAbility();
        if (enhanced)
            myCharacter.OnDamageTaken += OnDamageTaken;
    }

    public override void unsetCaster(Character caster)
    {
        myCharacter.characterStats.FireResist -= changeByValue;
        if (enhanced)
            myCharacter.OnDamageTaken -= OnDamageTaken;
    }

    public override void weakenedAbility()
    {
        changeByValue -= changeByValue/3;
        weakned = true;
    }
}

