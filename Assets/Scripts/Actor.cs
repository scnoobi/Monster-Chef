using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public abstract class Actor {
    public string name;
    public List<Ability> innateAbilities;

    public EventHandler OnDamageTaken;
}

