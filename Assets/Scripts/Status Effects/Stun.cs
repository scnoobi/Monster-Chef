using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Stun : StatusEffects
{
    Actor afflictedActor;
    float duration;

    public void applyEffect()
    {
        afflictedActor.topDownController.canAttack = false;
        afflictedActor.topDownController.canMove = false;
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
}
