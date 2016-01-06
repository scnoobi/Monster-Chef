using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SkillMenu : MonoBehaviour {

    public GameObject skillSlotPrefab;
    public bool foodSkillsMenu = false;

    int size;
    List<GameObject> slots = new List<GameObject>();
    Character character;
    SpritesLoader spriteLoader;

    // Use this for initialization
    void Awake () {
        spriteLoader = GameObject.Find("Loader").GetComponent<SpritesLoader>();
        Debug.Log(spriteLoader);
        if (this.gameObject.name.Equals("Food Skills"))
            foodSkillsMenu = true;
    }

    public void SetCharacter(Character character)
    {
        this.character = character;
    }

    public void updateSkillList()
    {
        if (foodSkillsMenu)
            size = character.getFoodAbilities().Count;
        else
            size = character.getCharAbilities().Count;
        for (int i = 0; i < size; i++)
        {
            GameObject skillSlot = (GameObject)Instantiate(skillSlotPrefab);
            slots.Add(skillSlot);
            skillSlot.transform.SetParent(transform);
            skillSlot.transform.localScale = transform.localScale;
            if (foodSkillsMenu)
                skillSlot.GetComponent<Image>().sprite = spriteLoader.getSkillSpriteWithName(character.getFoodAbilities()[i].realName);
            else
                skillSlot.GetComponent<Image>().sprite = spriteLoader.getSkillSpriteWithName(character.getCharAbilities()[i].realName);
        }
    }

	// Update is called once per frame
	void Update () {
	
	}

}
