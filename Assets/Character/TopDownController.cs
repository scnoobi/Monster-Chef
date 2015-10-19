using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownController : MonoBehaviour {

    public float maxSpeed = 10f;
    Vector2 direction = new Vector2(0f, 0f);
    public GameObject inventoryMenu;
    public GameObject cookingMenu;
    bool onAMenu = false;

    private Vector2 playerMovement;

    private Rigidbody2D myRig;
    private Inventory inv;

    void Start()
    {
        myRig = GetComponent<Rigidbody2D>();
        inv = inventoryMenu.GetComponent<Inventory>();
	}

	// Update is called once per frame
	void Update () {
        playerMovement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("do attack");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("interact");
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log(" Inventory");
            inventoryMenu.SetActive(!inventoryMenu.activeSelf);
            onAMenu = !onAMenu;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("open map");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("open cooking menu");
            cookingMenu.SetActive(!cookingMenu.activeSelf);
            onAMenu = !onAMenu;
        }

	}

    void FixedUpdate()
    {
        myRig.MovePosition(myRig.position + playerMovement.normalized * maxSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "PickUp")
        {
           if(inv.addItemToInventory(coll.gameObject.GetComponent<SpriteRenderer>().sprite, coll.gameObject.GetComponent<Item>()))
                Destroy(coll.gameObject);
        }
    }

}
