using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Stun : StatusEffects
{
    Actor afflictedActor;
    float duration;
    public TypeOfEffects typeOfStun;
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
        afflictedActor.topDownController.canAttack = false;
        afflictedActor.topDownController.canMove = false;
    }

    public StatusEffects clone()
    {
        Stun stun = new Stun();
        stun.duration = this.duration;
        return stun;
    }

    public int compare(StatusEffects statusEffects)
    {
        Stun comparing = (Stun)statusEffects;
        if (typeOfStun == comparing.typeOfStun)
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
