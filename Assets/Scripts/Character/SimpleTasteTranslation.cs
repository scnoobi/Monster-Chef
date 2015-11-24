using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class SimpleTasteTranslation : TasteToStats
{

    Character.stats TasteToStats.tasteToStats(Food food)
    {
        Character.stats statsToReturn = new Character.stats();
        statsToReturn.maxHP += food.foodTaste.umami;
        statsToReturn.attackSpeed += food.foodTaste.bitter;
        statsToReturn.movementSpeed += food.foodTaste.bitter;
        return statsToReturn;
    }
}
