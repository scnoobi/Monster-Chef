using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownController : MonoBehaviour {

    public float maxSpeed;    //speed of character
    public GameObject inventoryMenu;    //UI of inventory
    public GameObject cookingMenu;      //UI of cooking
    public GameObject mealPlanMenu;      //UI of mealPlan
    bool onAMenu = false;   //is any menu UI on

    private Vector2 playerMovement;

    private Rigidbody2D myRig;
    private Inventory inv;
    private Character character;

    void Start()
    {
        CharacterDatabase charDB = GameObject.Find("Databases").GetComponent<CharacterDatabase>();
        character = charDB.getCharacterById(0);
        character.Initialize();
        character.setController(this);
        myRig = GetComponent<Rigidbody2D>(); 
        inv = inventoryMenu.GetComponent<Inventory>(); 
	}

    public void setMaxSpeed(float maxSpeed) { this.maxSpeed = maxSpeed; }

    public Character getCharacter() { return character; }

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
            inventoryMenu.SetActive(!inventoryMenu.activeSelf);
            onAMenu = inventoryMenu.activeSelf || mealPlanMenu.activeSelf || cookingMenu.activeSelf;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            mealPlanMenu.SetActive(!mealPlanMenu.activeSelf);
            onAMenu = inventoryMenu.activeSelf || mealPlanMenu.activeSelf || cookingMenu.activeSelf;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("open map");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            cookingMenu.SetActive(!cookingMenu.activeSelf);
            onAMenu = inventoryMenu.activeSelf || mealPlanMenu.activeSelf || cookingMenu.activeSelf;
        }

        for (int i = 0; i < 10; ++i)
        {
            if (Input.GetKeyDown((KeyCode)48+i)){
                i = (i - 1) % 9;
                if (i < 0) i = 9;
                character.castCorrectAbility(i);
                break;
            }
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
            if (inv.addPickupToInventory(pickup)) //add the item pickup to the inventory
                Destroy(coll.gameObject); //destroy the pickup item on the ground
        }
    }
}
