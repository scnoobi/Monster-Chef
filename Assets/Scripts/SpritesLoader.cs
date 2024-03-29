﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpritesLoader : MonoBehaviour
{

    public Dictionary<string, Sprite> groundSprites = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> inventorySprites = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> abilitiesSprites = new Dictionary<string, Sprite>();

    // Use this for initialization
    void Start()
    {
        Sprite[] groundSheet = Resources.LoadAll<Sprite>("Sprites/Items/itemSprites");
        for(int i = 0; i < groundSheet.Length; i++)
        {
            groundSprites.Add(groundSheet[i].name, groundSheet[i]);
        }
        Sprite[] inventorySheet = Resources.LoadAll<Sprite>("Sprites/Items/itemSprites");
        for (int i = 0; i < inventorySheet.Length; i++)
        {
            inventorySprites.Add(inventorySheet[i].name, inventorySheet[i]);
        }
        Sprite[] abilitySheet = Resources.LoadAll<Sprite>("Sprites/Skills/abilities");
        for (int i = 0; i < abilitySheet.Length; i++)
        {
            abilitiesSprites.Add(abilitySheet[i].name, abilitySheet[i]);
        }
    }

    public Sprite getSpriteWithName( string name)
    {
        try {
            return groundSprites[name];
        }catch(KeyNotFoundException e)
        {
            Debug.Log("sprite with name "+ name + "not found");
            return null;
        }
    }

    public Sprite getSkillSpriteWithName(string name)
    {
        try
        {
            return abilitiesSprites[name];
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log("sprite with name " + name + "not found");
            return null;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
