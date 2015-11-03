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
        statsToReturn.maxHP += food.foodTaste.fat;
        statsToReturn.attackSpeed += food.foodTaste.sweetness;
        statsToReturn.movementSpeed += food.foodTaste.sweetness;
        return statsToReturn;
    }
}
