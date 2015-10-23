using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public int nextArea;
    public AreaGen area;
    public int i, j;
    public Connection.Direction chosenDirection;

	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
            NextArea();
    }

    //player entered the door, spawn the next area
    public void NextArea() { 
        area.TriggerNextArea(nextArea);
    }

	// Update is called once per frame
	void Update () {
	
	}
}
