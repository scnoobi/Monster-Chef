using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Slow : StatusEffects
{
    Actor afflictedActor;
    float duration;
    float slow;
    public TypeOfEffects typeOfSlow;
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
        
    }

    public StatusEffects clone()
    {
        Slow slow = new Slow();
        slow.duration = this.duration;
        return slow;
    }

    public int compare(StatusEffects statusEffects)
    {
        Slow comparing = (Slow)statusEffects;
        if (typeOfSlow == comparing.typeOfSlow)
            return -2;
        float slowOfStatus = slow /duration;
        float slowOfComparing = slow / duration;
        if (slowOfStatus == slowOfComparing)
            return 0;
        else if (slowOfStatus < slowOfComparing)
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

