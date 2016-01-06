using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum MovementStatus { Stunned, Rooted, Normal }
public abstract class Actor {
    public string name;
    public List<Ability> innateAbilities;
    public List<StatusEffects> affectedStatusEffects;
    public List<StatusEffects> onHitStatusEffects;
    public TopDownController topDownController;

    public EventHandler OnDamageTaken;
    public EventHandler OnFireDamageTaken;
    public EventHandler OnPoisonDamageTaken;
    public EventHandler OnIceDamageTaken;
    public abstract void TakeDamage(float damage);
    public abstract void TakeFireDamage(float damage);
    public abstract void TakePoisonDamage(float damage);
    public abstract void TakeIceDamage(float damage);
    public WorldTicker worldTicker;

    public void applyStatusEffect(StatusEffects statusEffect)
    {
        affectedStatusEffects.Add(statusEffect);
    }

    public void unapplyStatusEffect(StatusEffects statusEffect)
    {
        affectedStatusEffects.Remove(statusEffect);
    }

    public void registerTimedEvent(EventHandler timedEvent) {
        worldTicker.registerTimedEvent(timedEvent);
    }

    public void unregisterTimedEvent(EventHandler timedEvent)
    {
        worldTicker.unregisterTimedEvent(timedEvent);
    }
}

