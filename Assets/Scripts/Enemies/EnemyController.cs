using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D myRig;
    private Enemies enemy;

    void Start()
    {
        EnemyDatabase enemyDB = GameObject.Find("Databases").GetComponent<EnemyDatabase>();
        enemy = enemyDB.getEnemyByID(0); //This, with some work, can be passed to EnemyDatabase so it only runs once
        enemy.Initialize();
        enemy.SetController(this);
        myRig = GetComponent<Rigidbody2D>();
    }





}
