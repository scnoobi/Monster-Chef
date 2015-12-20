using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public abstract class Actor {
    public string name;
    public List<Ability> innateAbilities;
    public List<StatusEffects> statusEffects;

    public EventHandler OnDamageTaken;
    public abstract void TakeDamage(float damage);
    public WorldTicker worldTicker;

    public void registerTimedEvent(EventHandler timedEvent) {
        worldTicker.registerTimedEvent(timedEvent);
    }

    public void unregisterTimedEvent(EventHandler timedEvent)
    {
        worldTicker.unregisterTimedEvent(timedEvent);
    }
}

