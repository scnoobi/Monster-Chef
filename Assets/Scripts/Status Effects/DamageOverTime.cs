using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class DamageOverTime : StatusEffects
{
    float damage;
    float hitsPerSecond;
    Actor afflictedActor;
    float duration;

    float lastTick = 0f;
    float time;

    public void applyEffect()
    {
        afflictedActor.TakeDamage(damage);
    }

    void timedEvent(object sender, EventArgs e)
    {
        if (time - lastTick > hitsPerSecond)
        {
            applyEffect();
            lastTick = time;
        } 
    }

    public void setAfflicted(Actor actor)
    {
        actor.applyStatusEffect(this);
        afflictedActor = actor;
        actor.registerTimedEvent(timedEvent);
        time += Time.deltaTime;
    }

    public void unsetAfflicted(Actor actor)
    {
        actor.unapplyStatusEffect(this);
        afflictedActor = null;
        actor.unregisterTimedEvent(timedEvent);
    }
}

