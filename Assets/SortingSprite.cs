using UnityEngine;
using System.Collections;

public class SortingSprite : MonoBehaviour {

    private SpriteRenderer sprite;
    private int y;
    // Use this for initialization
    void Start () {
        sprite = GetComponent<SpriteRenderer>();

        if (sprite!=null)
        {
            sprite.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
        }
    }
	
	// Update is called once per frame
	void Update () {
        sprite.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
    }
}
