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
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        raycastCollisionDetection();
        Vector2 direction = new Vector2(h, v);
        transform.Translate(direction.normalized * maxSpeed * Time.deltaTime);
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

        RaycastHit2D[] hitUp, hitDwn, hitL, hitR;

        hitUp = Physics2D.RaycastAll(transform.position, Vector2.up, lookAhead);
        hitDwn = Physics2D.RaycastAll(transform.position, Vector2.down, lookAhead);
        hitL = Physics2D.RaycastAll(transform.position, Vector2.left, lookAhead);
        hitR = Physics2D.RaycastAll(transform.position, Vector2.right, lookAhead);
        
        Debug.DrawRay(transform.position, Vector2.up * lookAhead);
        Debug.DrawRay(transform.position, Vector2.down * lookAhead);
        Debug.DrawRay(transform.position, Vector2.left * lookAhead);
        Debug.DrawRay(transform.position, Vector2.right * lookAhead);

        foreach (RaycastHit2D hit in hitUp)
        {
            if (hit.collider != null && !hit.collider.isTrigger)
            {
                v = Mathf.Min(0, v);
            }
            else if (hit.collider != null && hit.collider.tag.Equals("Door"))
            {

                touchedDoor(hit);
            }
        }

        foreach (RaycastHit2D hit in hitDwn)
        {
            if (hit.collider != null && !hit.collider.isTrigger)
            {
                v = Mathf.Max(0, v);
            }
            else if (hit.collider != null && hit.collider.tag.Equals("Door"))
            {

                touchedDoor(hit);
            }
        }

        foreach (RaycastHit2D hit in hitL)
        {
            if (hit.collider != null && !hit.collider.isTrigger)
            {
                h = Mathf.Max(0, h);
            }
            else if (hit.collider != null && hit.collider.tag.Equals("Door"))
            {

                touchedDoor(hit);
            }
        }

        foreach (RaycastHit2D hit in hitR)
        {
            if (hit.collider != null && !hit.collider.isTrigger)
            {
                h = Mathf.Min(0, h);
            }
            else if (hit.collider != null && hit.collider.tag.Equals("Door"))
            {
                touchedDoor(hit);
            }
        }

        /*
        Physics2D.Raycast(transform.position, Quaternion.AngleAxis(45, Vector2.up) * Vector2.up, lookAhead);
        Physics2D.Raycast(transform.position, Quaternion.AngleAxis(-45, Vector2.up) * Vector2.up, lookAhead);
        Physics2D.Raycast(transform.position, Quaternion.AngleAxis(45, Vector2.down) * Vector2.down, lookAhead);
        Physics2D.Raycast(transform.position, Quaternion.AngleAxis(-45, Vector2.down) * Vector2.down, lookAhead);
         * */
    }
}
