using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillMenu : MonoBehaviour {

    public GameObject skillSlotPrefab;
    public bool foodSkillsMenu = false;

    int size;
    List<GameObject> slots = new List<GameObject>();

    // Use this for initialization
    void Start () {
        if (foodSkillsMenu)
        {
            size = 0; //TODO: get number from character ammount of skills
        }
        else
            size = 2;


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
