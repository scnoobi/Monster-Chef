using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class Ability
{
   public bool isActiveAbility;
   public string realName;
   protected bool enhanced;
   protected bool weakned;

    public Ability() {
        if (enhanced)
            enhancedAbility();
        if (weakned)
            enhancedAbility();
    }

   public abstract void castAbility();
   public abstract void setCaster(Character caster);
   public abstract void unsetCaster(Character caster);
   public abstract void weakenedAbility();
   public abstract void enhancedAbility();
}
