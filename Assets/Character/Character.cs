using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

    public float maxHP;
    public float currHP;
    public float maxHunger;
    public float currHunger;

    public float attackSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if(Input.GetButtonDown("Fire1")){
            Debug.Log("do attack");
        }

        if(Input.GetKeyDown(KeyCode.E)){
            Debug.Log("do attack");
        }
        if(Input.GetKeyDown(KeyCode.I)){
            Debug.Log("open Inventory");
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("open map");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("open cooking menu");
        }
	}
}
