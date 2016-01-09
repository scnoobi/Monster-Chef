using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : TopDownController
{
    private Enemies enemy;
    public GameObject healthPrefab;
    public Canvas canvas;

    private GameObject healthPanel;
    private Image healthBar;
    private Text damagePopup;

    void Start()
    {
        worldTicker = GameObject.Find("GameManager").GetComponent<WorldTicker>();
        canvas = GameObject.Find("HealthCanvas").GetComponent<Canvas>();

        EnemyDatabase enemyDB = GameObject.Find("Databases").GetComponent<EnemyDatabase>();
        enemy = enemyDB.getEnemyByID(0); //This, with some work, can be passed to EnemyDatabase so it only runs once
        enemy.Initialize();
        enemy.SetController(this);
        myRig = GetComponent<Rigidbody2D>();

        initHealthBar();
    }

    void initHealthBar()
    {
        healthPanel = Instantiate(healthPrefab);
        healthPanel.transform.SetParent(canvas.transform, false);
        healthPanel.name = this.name;

        healthPanel.GetComponentInChildren<Text>().text = enemy.name;

        healthBar = healthPanel.GetComponentInChildren<Image>();
        healthBar.name = this.name;

        damagePopup = healthPanel.gameObject.GetComponentsInChildren<Text>(true)[1];
    }

    public void FloatingDamage(float damage, TypeOfEffects typeOfEffects)
    {
        Text floatDmg = Instantiate(damagePopup);
        floatDmg.gameObject.SetActive(true);
        floatDmg.transform.SetParent(healthPanel.transform, false);
        floatDmg.text = damage.ToString();
        floatDmg.color = Color.white;
        if(typeOfEffects == TypeOfEffects.fire)
            floatDmg.color = Color.red;
    }

    public void setHealthBarValue(float value) {
        healthBar.fillAmount = value;
    }

    public Enemies getEnemy()
    {
        return enemy;
    }

    void Update()
    {
        Vector3 worldPos = new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        healthPanel.transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
    }

}
