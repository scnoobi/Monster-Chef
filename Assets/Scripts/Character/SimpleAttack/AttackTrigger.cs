using UnityEngine;
using System.Collections;

public class AttackTrigger : MonoBehaviour {

    MyCharacterController controller;
    Character character;
	// Use this for initialization
	void Start () {
        controller = transform.GetComponentInParent<MyCharacterController>();
    }

    public void setCharacter(Character character)
    {
        this.character = character;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Enemy" && character.attacksMelee && controller.animator.GetBool("attacking"))
        {
            coll.gameObject.GetComponent<EnemyController>().getEnemy().TakeDamage(character.characterStats.AttackDamage);
            for (int i = 0; i < character.onHitStatusEffects.Count; i++)
            {
                if(character.onHitStatusEffects[i].ChanceOfApplying > Random.value*100)
                character.onHitStatusEffects[i].clone().setAfflicted(coll.gameObject.GetComponent<EnemyController>().getEnemy());
            }
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
