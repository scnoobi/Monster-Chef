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
        statsToReturn.MaxHP += food.foodTaste.umami;
        statsToReturn.AttackSpeed += food.foodTaste.bitter;
        statsToReturn.MovementSpeed += food.foodTaste.bitter;
        return statsToReturn;
    }
}
