﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class Ability
{
   public bool isActiveAbility;
   public abstract void castAbility();
   public abstract void setCaster(Character caster);
   public abstract void weakenedAbility();
   public abstract void enhancedAbility();
}
