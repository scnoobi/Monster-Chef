using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class MyCharacterController : TopDownController {

    public GameObject inventoryMenu;    //UI of inventory
    public GameObject cookingMenu;      //UI of cooking
    public GameObject mealPlanMenu;      //UI of mealPlan
    bool onAMenu = false;   //is any menu UI on

    private Inventory inv;
    private Character character;

    void Start()
    {
        //init controller stuff
        myRig = GetComponent<Rigidbody2D>();
        inv = inventoryMenu.GetComponent<Inventory>();
        animator = GetComponent<Animator>();
        facingLeft = false;

        //init character stuff
        CharacterDatabase charDB = GameObject.Find("Databases").GetComponent<CharacterDatabase>();
        character = charDB.getCharacterById(6);
        character.Initialize(this);
        GetComponentInChildren<AttackTrigger>().setCharacter(character);
    }

    public CookingMenuSlot getCookingSlot() {
        return cookingMenu.GetComponentsInChildren<CookingMenuSlot>(true)[0];
    }

    public void setMaxSpeed(float maxSpeed) { this.maxSpeed = maxSpeed; }

    public Character getCharacter() { return character; }

    public bool CanMove() { return canMove && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"); }

    public bool CanAttack() { return canAttack && !onAMenu; }

    // Update is called once per frame
    void Update () {
        movementVector = new Vector2(0, 0);
        if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical")) && CanMove())
        {
            movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            animator.SetBool("walking", true);
            SetCorrectAnimationDirection(movementVector);
        }
        else if(animator.GetBool("walking"))
        {
            animator.SetBool("walking", false);
        }

        //handle input
        if (Input.GetButtonDown("Fire1") && CanAttack())
        {
            animator.SetBool("attacking", true);
            character.Attack();
        }
        else if(animator.GetBool("attacking") && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")){
            animator.SetBool("attacking", false);
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

        if (Input.GetKeyDown(KeyCode.T))
        {
            character.TakeDamage(1f);
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
        myRig.MovePosition(myRig.position + movementVector.normalized * maxSpeed * Time.deltaTime); //move player 
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

    void SetCorrectAnimationDirection(Vector2 input)
    {
        animator.SetFloat("inputX", input.x);
        animator.SetFloat("inputY", input.y);
        if (input.x < 0 && !facingLeft)
        {
            facingLeft = true;
            Flip();
        }
        else if (input.x > 0 && facingLeft) {
            facingLeft = false;
            Flip();
        }
    }

    void Flip()
    {
        Vector3 currScale = transform.localScale;
        currScale.x *= -1;
        transform.localScale = currScale;

    }
}
