using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingTextMovement : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}
	
    public void DestroyText()
    {
        GameObject.Destroy(this.gameObject);
    }

	// Update is called once per frame
	void Update () {
	}
}
