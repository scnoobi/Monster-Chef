﻿using UnityEngine;
using System.Collections;

public class PickupFood : PickupItem {

    public string name;
    private ItemDatabase itemDB;
    private Food food;
    public enum typesOfFood { meat, fish, veggies }
    private SpritesLoader spriteLoader;
    
    override public Item getItem()
    {
        return getFood();
    }

    public void Start() {
        if (food == null)
        {
            spriteLoader = GameObject.Find("Loader").GetComponent<SpritesLoader>();
            itemDB = GameObject.Find("itemDB").GetComponent<ItemDatabase>();
            food = (Food)itemDB.getItemByName(name);
            if (inventorySprite == null)
            {
                Debug.Log("1");
                inventorySprite = spriteLoader.getSpriteWithName(food.realName);
            }
            if (gameObject.GetComponent<SpriteRenderer>().sprite == null)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = spriteLoader.getSpriteWithName(food.realName);
                Debug.Log("2");
            }
            sizeX = food.sizeX;
            sizeY = food.sizeY;
        }
        gameObject.GetComponent<SpriteRenderer>().sprite = spriteLoader.getSpriteWithName(food.realName);
    }

    public Food getFood()
    {
        return food;
    }

    public void Initialize(string realName, Food.taste foodTaste, float timeToCook, Food.cookingType currentCookingType)
   {
        spriteLoader = GameObject.Find("Loader").GetComponent<SpritesLoader>();
        itemDB = GameObject.Find("itemDB").GetComponent<ItemDatabase>();
        food = (Food)itemDB.getItemByName(realName);
        this.name = realName;
        if (inventorySprite == null)
        {
            Debug.Log("1");
            inventorySprite = spriteLoader.getSpriteWithName(food.realName);
        }
        gameObject.GetComponent<SpriteRenderer>().sprite = spriteLoader.getSpriteWithName(food.realName);
        Debug.Log(spriteLoader.getSpriteWithName(food.realName));
        sizeX = food.sizeX;
        sizeY = food.sizeY;
    }
}
