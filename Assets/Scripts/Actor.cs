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

    public bool applyStatusEffect(StatusEffects statusEffect)
    {
 
        return HandleEffects(statusEffect);
    }

    public void unapplyStatusEffect(StatusEffects statusEffect)
    {
        affectedStatusEffects.Remove(statusEffect);
    }

    public void registerTimedEvent(EventHandler<TimedEventArgs> timedEvent) {
        topDownController.worldTicker.registerTimedEvent(timedEvent);
    }

    public void unregisterTimedEvent(EventHandler<TimedEventArgs> timedEvent)
    {
        topDownController.worldTicker.unregisterTimedEvent(timedEvent);
    }

    public bool HandleEffects(StatusEffects statusEffect)
    {
        for(int i = affectedStatusEffects.Count-1; i >= 0; i--)
        {
            if(statusEffect.GetType() == affectedStatusEffects[i].GetType())
            {
                int result = statusEffect.compare(affectedStatusEffects[i]);
                if (result == -2) //not even same elemental type
                    continue;
                else if (result == -1) //weaker so give up
                {
                    return false;
                }
                else if (result == 0) // equal so just refresh
                {
                    affectedStatusEffects[i].refresh();
                    return false;
                }
                else //stronger so replate
                {
                    affectedStatusEffects.Add(statusEffect);
                    affectedStatusEffects.Remove(affectedStatusEffects[i]);
                    return true;
                }
            }
        }
        affectedStatusEffects.Add(statusEffect);
        return true;
    }



}

