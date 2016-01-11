using UnityEngine;
using System.Collections;

public class EnvironmentalDamage : MonoBehaviour {
    public bool instakill = false;
    public float damage = 2;
    public TypeOfEffects typeOfDamage = TypeOfEffects.fire;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag.Equals("Enemy"))
        {
            coll.gameObject.GetComponent<EnemyController>().getEnemy().TakeDamage(damage);
        }

        if (coll.gameObject.tag.Equals("Player"))
        {
            coll.gameObject.GetComponent<MyCharacterController>().getCharacter().TakeDamage(damage);
        }
    }
}
