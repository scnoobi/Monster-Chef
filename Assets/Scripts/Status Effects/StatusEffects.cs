using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum TypeOfEffects {none, fire, ice, poison, chilled, slowed, frozen, rooted, stunned, paralyzed};
public interface StatusEffects
{
    float ChanceOfApplying { get; set; }

    void applyEffect();
    StatusEffects clone();
    void setAfflicted(Actor actor);
    void unsetAfflicted(Actor actor);
    int compare(StatusEffects statusEffects); //-2 diff type, -1 worse, 0 equal, 1 better
    void refresh();
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
