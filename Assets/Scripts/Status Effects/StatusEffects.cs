using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface StatusEffects
{
    void applyEffect();
    void setAfflicted(Actor actor);
    void unsetAfflicted(Actor actor);
}


/*
LIST:

    DOTS:
        poisoned
        burned

    SLOW:
        chilled
        slowed

    ROOTED:
        frozen
        rooted

    STUNS:
        stunned
        paralyzed
*/
