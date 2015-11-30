using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MealPlan : MonoBehaviour {

    public GameObject mealPSlotPrefab;
    List<MealPlanSlot> slots = new List<MealPlanSlot>();
    int numberOfMeals = 3; //will depend on the character
    public GameObject inventoryObj;
    Character character;

	void Start () {
        for (int i = 0; i < numberOfMeals; i++)
        {
            GameObject slot = (GameObject)Instantiate(mealPSlotPrefab);
            slots.Add(slot.GetComponent<MealPlanSlot>());
            slot.GetComponent<MealPlanSlot>().inventory = inventoryObj.GetComponent<Inventory>();
            slot.transform.SetParent(this.transform);
            slot.transform.localScale = this.transform.localScale;
        }
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<TopDownController>().getCharacter();
    }


   public void ConsumeMealPlan() {
        List<Food> foodOnMealPlan = new List<Food>();
        for(int i = 0; i < slots.Count;i++){
            if (slots[i].currentItem != null)
                foodOnMealPlan.Add((Food)slots[i].currentItem.getItem());
        }
        if (foodOnMealPlan.Count == 0)
            return;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].currentItem != null)
            {
                Destroy(slots[i].currentItem.gameObject);
                slots[i].currentItem = null;
                slots[i].occupied = false;
            }
        }
        character.ConsumeMeals(foodOnMealPlan);
    }

}
