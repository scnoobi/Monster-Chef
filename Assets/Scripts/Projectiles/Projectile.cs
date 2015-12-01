using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    Vector3 originPos;

	// Use this for initialization
	void Start () {
        originPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
	    if(transform.position.y == originPos.y)
        {
            Activate();
        }
	}

    void Activate()
    {

    }
}
