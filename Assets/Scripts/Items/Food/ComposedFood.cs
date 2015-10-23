using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComposedFood : Food
{
    List<Food> foodParts;

	// Use this for initialization
	void Start () {
	
	}
	
    public void addFood(Food part){
        foodParts.Add(part);
        foodTaste.complexTaste(part.foodTaste);
    }

	// Update is called once per frame
	void Update () {
	
	}
}
