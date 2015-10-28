using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownController : MonoBehaviour {

    public float maxSpeed = 10f;    //speed of character
    public GameObject inventoryMenu;    //UI of inventory
    public GameObject cookingMenu;      //UI of cooking
    bool onAMenu = false;   //is any menu UI on

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
        playerMovement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //get Movement input and set it into a directio vector


        //handle input
        if (Input.GetButtonDown("Fire1") && !onAMenu)
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
        myRig.MovePosition(myRig.position + playerMovement.normalized * maxSpeed * Time.deltaTime); //move player 
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "PickUp")
        {
            PickupItem pickup = coll.gameObject.GetComponent<PickupItem>();
            if (inv.addItemToInventory(pickup)) //add the item pickup to the inventory
                Destroy(coll.gameObject); //destroy the pickup item on the ground
        }
    }
}
