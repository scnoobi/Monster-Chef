using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpritesLoader : MonoBehaviour
{

    public Dictionary<string, Sprite> groundSprites = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> inventorySprites = new Dictionary<string, Sprite>();

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
    }

    public Sprite getSpriteWithName( string name)
    {
        return groundSprites[name];
    }

    // Update is called once per frame
    void Update()
    {

    }
}
