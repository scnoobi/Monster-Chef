using UnityEngine;
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
                if(spriteLoader.getSpriteWithName(food.realName) != null)
                    inventorySprite = spriteLoader.getSpriteWithName(food.realName);
            }
            if (gameObject.GetComponent<SpriteRenderer>().sprite == null)
            {
                if (spriteLoader.getSpriteWithName(food.realName) != null)
                    gameObject.GetComponent<SpriteRenderer>().sprite = spriteLoader.getSpriteWithName(food.realName);
            }
            sizeX = food.sizeX;
            sizeY = food.sizeY;
        }
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
            if (spriteLoader.getSpriteWithName(food.realName) != null)
                inventorySprite = spriteLoader.getSpriteWithName(food.realName);
        }
        if (gameObject.GetComponent<SpriteRenderer>().sprite == null)
        {
            if (spriteLoader.getSpriteWithName(food.realName) != null)
                gameObject.GetComponent<SpriteRenderer>().sprite = spriteLoader.getSpriteWithName(food.realName);
        }
        Debug.Log(gameObject.GetComponent<SpriteRenderer>().sprite);
        sizeX = food.sizeX;
        sizeY = food.sizeY;
    }
}
