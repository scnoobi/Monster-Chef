using UnityEngine;
using System.Collections;

public class TopDownController : MonoBehaviour {

    public float maxSpeed = 10f;
    Vector2 direction = new Vector2(0f, 0f);

    private bool colliding = false;
    private float lookAhead = 0.5f;

    float h;
    float v;
    private bool justWentTroughDoor = false;
    private float doorWalkingCooldown = .5f;
    private float doorWalkingTimer = 0f;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
        lookAhead = maxSpeed * Time.deltaTime;
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        if (v != 0 || h != 0 )
        {
            raycastCollisionDetection();
            Vector2 direction = new Vector2(h, v);
            transform.Translate(direction.normalized * maxSpeed * Time.deltaTime);
        }
	}

    void touchedDoor(RaycastHit2D hit)
    {
        if (!justWentTroughDoor)
        {
            hit.collider.GetComponent<Door>().NextArea();
            justWentTroughDoor = true;
        }
    }

    void raycastCollisionDetection() {
        if (justWentTroughDoor)
            if (doorWalkingCooldown < Time.deltaTime + doorWalkingTimer)
            {
                justWentTroughDoor = false;
                doorWalkingTimer = 0f;
            }
            else
                doorWalkingTimer += Time.deltaTime;

        RaycastHit2D[] hitUp, hitDwn, hitL, hitR, hitUL, hitUR, hitDL, hitDR;
        lookAhead = maxSpeed * Time.deltaTime;

        Vector3 startUp = new Vector3(GetComponent<BoxCollider2D>().bounds.center.x, GetComponent<BoxCollider2D>().bounds.max.y);
        Vector3 startDown =  new Vector3(GetComponent<BoxCollider2D>().bounds.center.x, GetComponent<BoxCollider2D>().bounds.min.y);
        Vector3 startLeft = new Vector3(GetComponent<BoxCollider2D>().bounds.min.x, GetComponent<BoxCollider2D>().bounds.center.y);
        Vector3 startRight =  new Vector3(GetComponent<BoxCollider2D>().bounds.max.x, GetComponent<BoxCollider2D>().bounds.center.y);


        hitUp = Physics2D.RaycastAll( startUp, Vector2.up, lookAhead );
        hitDwn = Physics2D.RaycastAll(startDown, Vector2.down, lookAhead );
        hitL = Physics2D.RaycastAll(startLeft, Vector2.left, lookAhead );
        hitR = Physics2D.RaycastAll(startRight, Vector2.right, lookAhead );

        Debug.DrawRay(startUp, Vector2.up * (lookAhead ), Color.red);
        Debug.DrawRay(startDown, Vector2.down * (lookAhead ), Color.red);
        Debug.DrawRay(startLeft, Vector2.left * (lookAhead ), Color.red);
        Debug.DrawRay(startRight, Vector2.right * (lookAhead ), Color.red);


        /*
hitUL = Physics2D.RaycastAll(transform.position, new Vector2(0.5f, 0.5f).normalized, lookAhead);
hitDL = Physics2D.RaycastAll(transform.position, new Vector2(-0.5f, 0.5f).normalized, lookAhead);
hitUR = Physics2D.RaycastAll(transform.position, new Vector2(0.5f, -0.5f).normalized, lookAhead);
hitDR = Physics2D.RaycastAll(transform.position, new Vector2(-0.5f, -0.5f).normalized, lookAhead);
 *         Debug.DrawRay(transform.position, new Vector2(0.5f, 0.5f).normalized * lookAhead, Color.red);
Debug.DrawRay(transform.position, new Vector2(-0.5f, 0.5f).normalized * lookAhead, Color.red);
Debug.DrawRay(transform.position, new Vector2(0.5f, -0.5f).normalized * lookAhead, Color.red);
Debug.DrawRay(transform.position, new Vector2(-0.5f, -0.5f).normalized * lookAhead, Color.red);
*/

        foreach (RaycastHit2D hit in hitUp)
        {
            if (!hit.collider.tag.Equals("Player") && hit.collider != null && !hit.collider.isTrigger )
            {
                v = Mathf.Min(0, v);
            }
            else if (!hit.collider.tag.Equals("Player") && hit.collider != null && hit.collider.tag.Equals("Door"))
            {

                touchedDoor(hit);
            }
        }

        foreach (RaycastHit2D hit in hitDwn)
        {
            if (!hit.collider.tag.Equals("Player") && hit.collider != null && !hit.collider.isTrigger)
            {
                v = Mathf.Max(0, v);
            }
            else if (!hit.collider.tag.Equals("Player") && hit.collider != null && hit.collider.tag.Equals("Door"))
            {

                touchedDoor(hit);
            }
        }

        foreach (RaycastHit2D hit in hitL)
        {
            if (!hit.collider.tag.Equals("Player") && hit.collider != null && !hit.collider.isTrigger)
            {
                h = Mathf.Max(0, h);
            }
            else if (!hit.collider.tag.Equals("Player") && hit.collider != null && hit.collider.tag.Equals("Door"))
            {

                touchedDoor(hit);
            }
        }

        foreach (RaycastHit2D hit in hitR)
        {
            if (!hit.collider.tag.Equals("Player") && hit.collider != null && !hit.collider.isTrigger)
            {
                h = Mathf.Min(0, h);
            }
            else if (!hit.collider.tag.Equals("Player") && hit.collider != null && hit.collider.tag.Equals("Door"))
            {
                touchedDoor(hit);
            }
        }

    }
}
