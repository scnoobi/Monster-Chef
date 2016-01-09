using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class DamageOverTime : StatusEffects
{
    public float damage;
    public float hitsPerSecond;
    public float duration;
    Actor afflictedActor;
    public TypeOfEffects typeOfDamage;
    private float chanceOfApplying;

    private float lastTick = 0f;
    private float time = 0f;
    private int hitsDone = 0;

    public float ChanceOfApplying
    {
        get
        {
            return chanceOfApplying;
        }

        set
        {
            chanceOfApplying = value;
        }
    }

    public void applyEffect()
    {
        if (typeOfDamage == TypeOfEffects.fire)
            afflictedActor.TakeFireDamage(damage);
        else if (typeOfDamage == TypeOfEffects.ice)
            afflictedActor.TakeIceDamage(damage);
        else if (typeOfDamage == TypeOfEffects.poison)
            afflictedActor.TakePoisonDamage(damage);
    }

    void timedEvent(object sender, TimedEventArgs e)
    {
        time = e.Time;
        if (time - lastTick > hitsPerSecond && hitsDone < (duration/hitsPerSecond))
        {
            applyEffect();
            lastTick = time;
            hitsDone ++;
        } else if (hitsDone > (duration / hitsPerSecond))
        {
            unsetAfflicted(afflictedActor);
        }
    }

    public void setAfflicted(Actor actor)
    {
        if (actor.applyStatusEffect(this))
        {
            afflictedActor = actor;
            actor.registerTimedEvent(timedEvent);
        }
    }

    public void unsetAfflicted(Actor actor)
    {
        actor.unapplyStatusEffect(this);
        afflictedActor = null;
        actor.unregisterTimedEvent(timedEvent);
    }

    public StatusEffects clone()
    {
        DamageOverTime dot = new DamageOverTime();
        dot.damage = this.damage;
        dot.hitsPerSecond = this.hitsPerSecond;
        dot.duration = this.duration;
        dot.typeOfDamage = this.typeOfDamage;
        return dot;
    }

    public int compare(StatusEffects statusEffects)
    {
        DamageOverTime comparing = (DamageOverTime)statusEffects;
        if (typeOfDamage != comparing.typeOfDamage)
            return -2;
        float damageOfStatus = damage * (duration / hitsPerSecond);
        float damageOfComparing = comparing.damage * (comparing.duration / comparing.hitsPerSecond);
        if (damageOfStatus == damageOfComparing)
            return 0;
        else if (damageOfStatus < damageOfComparing)
            return -1;
        else return 1;
    }

    public void refresh()
    {
        lastTick = 0f;
        time = 0f;
        hitsDone = 0;
    }

}

