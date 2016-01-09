using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Rooted : StatusEffects
{
    Actor afflictedActor;
    float duration;
    public TypeOfEffects typeOfRoot;
    public float chanceOfApplying;

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
        throw new NotImplementedException();
    }

    public StatusEffects clone()
    {
        Rooted rooted = new Rooted();
        rooted.duration = this.duration;
        return rooted;
    }

    public int compare(StatusEffects statusEffects)
    {
        Rooted comparing = (Rooted)statusEffects;
        if (typeOfRoot == comparing.typeOfRoot)
            return -2;
        float durationOfStatus = duration;
        float durationOfComparing = comparing.duration;
        if (durationOfStatus == durationOfComparing)
            return 0;
        else if (durationOfStatus < durationOfComparing)
            return -1;
        else return 1;
    }

    public void setAfflicted(Actor actor)
    {
        actor.applyStatusEffect(this);
        afflictedActor = actor;
    }

    public void unsetAfflicted(Actor actor)
    {
        actor.unapplyStatusEffect(this);
        afflictedActor = null;
    }

    public void refresh()
    {
        throw new NotImplementedException();
    }
}

