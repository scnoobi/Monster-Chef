using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class SpicyTasteTranslation : TasteToStats
{

    Character.Stats TasteToStats.tasteToStats(Food food)
    {
        Character.Stats statsToReturn = new Character.Stats();
        statsToReturn.MaxHP += food.foodTaste.umami;
        statsToReturn.CurrHP += food.foodTaste.spicy;
        statsToReturn.AttackDamage += food.foodTaste.spicy;
        statsToReturn.AttackSpeed += food.foodTaste.spicy;
        statsToReturn.MovementSpeed += food.foodTaste.spicy;
        return statsToReturn;
    }
}
