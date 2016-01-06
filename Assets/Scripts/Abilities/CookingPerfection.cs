using System;
using UnityEngine;

class CookingPerfection : Ability
{
    Character myCharacter;

    public override void castAbility()
    {
        myCharacter.getController().getCookingSlot().canBurn = false;
    }

    public override void enhancedAbility()
    {
    }

    public override void setCaster(Character caster)
    {
        myCharacter = caster;
        castAbility();
    }

    public override void unsetCaster(Character caster)
    {
        myCharacter.getController().getCookingSlot().canBurn = false;
    }

    public override void weakenedAbility()
    {
    }
}

