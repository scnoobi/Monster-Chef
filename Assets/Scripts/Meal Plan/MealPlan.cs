using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MealPlan : MonoBehaviour {

    public GameObject mealPSlotPrefab;
    List<GameObject> slots = new List<GameObject>();
    int numberOfMeals = 3; //will depend on the character
    public GameObject inventoryObj;
    Inventory inventory;

	void Start () {
        for (int i = 0; i < numberOfMeals; i++)
        {
            GameObject slot = (GameObject)Instantiate(mealPSlotPrefab);
            slots.Add(slot);
            slot.transform.SetParent(this.transform);
            slot.transform.localScale = this.transform.localScale;
        }
	}


    void ConsumeMealPlan() { 
    
    }

}
