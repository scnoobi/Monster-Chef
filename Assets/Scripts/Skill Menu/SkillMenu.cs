using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillMenu : MonoBehaviour {

    public GameObject skillSlotPrefab;
    public bool foodSkillsMenu = false;

    int size;
    List<GameObject> slots = new List<GameObject>();
    Character character;

    // Use this for initialization
    void Start () {
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<TopDownController>().getCharacter();
        if (foodSkillsMenu)
        {
            size = character.getFoodAbilities().Count;
        }
        else
            size = 2;

    }

    public void updateSkillList()
    {
        Debug.Log("dsd");
        size = character.getFoodAbilities().Count; ;
        for (int i = 0; i < size; i++)
        {
            GameObject skillSlot = (GameObject)Instantiate(skillSlotPrefab);
            slots.Add(skillSlot);
            skillSlot.transform.SetParent(transform);
            skillSlot.transform.localScale = transform.localScale;
        }
    }

	// Update is called once per frame
	void Update () {
	
	}

}
