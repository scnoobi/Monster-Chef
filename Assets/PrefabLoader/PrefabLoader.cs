using UnityEngine;
using System.Collections;

public class PrefabLoader : MonoBehaviour
{

    public GameObject[] forestFillers;

	// Use this for initialization
	void Start () {
        forestFillers = Resources.LoadAll<GameObject>("GameObject/FillerObjects/Forest");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
