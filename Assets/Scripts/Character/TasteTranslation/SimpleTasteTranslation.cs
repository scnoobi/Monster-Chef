using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class SimpleTasteTranslation : TasteToStats
{

    Character.Stats TasteToStats.tasteToStats(Food food)
    {
        Character.Stats statsToReturn = new Character.Stats();
        statsToReturn.maxHP += food.foodTaste.umami;
        statsToReturn.attackSpeed += food.foodTaste.bitter;
        statsToReturn.movementSpeed += food.foodTaste.bitter;
        return statsToReturn;
    }
}
