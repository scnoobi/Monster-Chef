using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class TopDownController : MonoBehaviour
{
    protected Rigidbody2D myRig;

    public Animator animator;
    protected bool facingLeft;

    public bool canMove = true;
    public bool canAttack = true;
    protected Vector2 movementVector;
    public float maxSpeed;
}

