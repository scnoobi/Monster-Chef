using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    ProjectileMovement projectileMovement;
    public Transform shooter;

    // Use this for initialization
    void Start () {
        projectileMovement = new Orbital(shooter, this);
    }
	
	// Update is called once per frame
	void Update () {
        projectileMovement.MoveProjectile();
    }

    public void Activate()
    {
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collided)
    {
         if(collided.transform != shooter && !collided.tag.Equals("PickUp"))
        {
            Activate();
        }
    }
}
