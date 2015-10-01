using UnityEngine;
using System.Collections;

public class TopDownController : MonoBehaviour {

    public float maxSpeed = 10f;
    Vector2 direction = new Vector2(0f, 0f);

    private bool colliding = false;
    private float lookAhead = 0.5f;

    float h;
    float v;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        raycastCollisionDetection();
        Vector2 direction = new Vector2(h, v);
        transform.Translate(direction.normalized * maxSpeed * Time.deltaTime);
	}


    void raycastCollisionDetection() {
        RaycastHit2D hitUp, hitDwn, hitL, hitR;

        hitUp = Physics2D.Raycast(transform.position, Vector2.up, lookAhead);
        hitDwn = Physics2D.Raycast(transform.position, Vector2.down, lookAhead);
        hitL = Physics2D.Raycast(transform.position, Vector2.left, lookAhead);
        hitR = Physics2D.Raycast(transform.position, Vector2.right, lookAhead);
        
        Debug.DrawRay(transform.position, Vector2.up * lookAhead);
        Debug.DrawRay(transform.position, Vector2.down * lookAhead);
        Debug.DrawRay(transform.position, Vector2.left * lookAhead);
        Debug.DrawRay(transform.position, Vector2.right * lookAhead);

        if (hitUp.collider != null && !hitUp.collider.isTrigger)
        {
            Debug.Log("U");
            /*
            float distance = Mathf.Abs(hitUp.point.y - transform.position.y);
            float heightError = GetComponent<SpriteRenderer>().bounds.size.y/2 - distance;
            if (heightError > 0)*/
                v = Mathf.Min(0, v);
        }

        if (hitDwn.collider != null && !hitDwn.collider.isTrigger)
        {
            Debug.Log("D");
            /*
            float distance = Mathf.Abs(hitUp.point.y - transform.position.y);
            float heightError = GetComponent<SpriteRenderer>().bounds.size.y/2 - distance;
            if (heightError > 0)*/
                v = Mathf.Max(0, v);
        }

        if (hitL.collider != null && !hitL.collider.isTrigger)
        {
            Debug.Log("L");
           /* float distance = Mathf.Abs(hitL.point.x - transform.position.x);
            float heightError = GetComponent<SpriteRenderer>().bounds.size.x / 2 - distance;
            if (heightError > 0)*/
            h = Mathf.Max(0, h);
        }

        if (hitR.collider != null && !hitR.collider.isTrigger)
        {
            Debug.Log("R");
            /* float distance = Mathf.Abs(hitL.point.x - transform.position.x);
            float heightError = GetComponent<SpriteRenderer>().bounds.size.x / 2 - distance;
            if (heightError > 0)*/
                
                h = Mathf.Min(0, h);
        }

        /*
        Physics2D.Raycast(transform.position, Quaternion.AngleAxis(45, Vector2.up) * Vector2.up, lookAhead);
        Physics2D.Raycast(transform.position, Quaternion.AngleAxis(-45, Vector2.up) * Vector2.up, lookAhead);
        Physics2D.Raycast(transform.position, Quaternion.AngleAxis(45, Vector2.down) * Vector2.down, lookAhead);
        Physics2D.Raycast(transform.position, Quaternion.AngleAxis(-45, Vector2.down) * Vector2.down, lookAhead);
         * */
    }
}
