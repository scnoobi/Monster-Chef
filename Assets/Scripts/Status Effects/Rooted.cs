﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Rooted : StatusEffects
{
    Actor afflictedActor;
    float duration;

    public void applyEffect()
    {
        throw new NotImplementedException();
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

