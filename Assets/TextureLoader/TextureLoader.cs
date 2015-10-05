using UnityEngine;
using System.Collections;

public class TextureLoader : MonoBehaviour {

    public Sprite[] forestFillers;

	// Use this for initialization
	void Start () {
        forestFillers = Resources.LoadAll<Sprite>("Textures/FillerObjects/Forest");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
