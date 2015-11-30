using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

public class ShootAbility : Ability
{
    public int numberOfProjectiles;
    public string projectileName;

    GameObject projectileToShoot;
    Transform character;
    private const string PATH_TO_PROJECTILES = "GameObject/Projectiles/";

    public ShootAbility() {
        projectileToShoot = (GameObject)Resources.Load<GameObject>(PATH_TO_PROJECTILES + projectileName);
    }

    public ShootAbility(int nProjectiles)
    {
        numberOfProjectiles = nProjectiles;
        projectileToShoot = (GameObject)Resources.Load<GameObject>(PATH_TO_PROJECTILES + projectileName);
    }

    [JsonConstructor]
    public ShootAbility(int nProjectiles, string projectileName)
    {
        this.projectileName = projectileName;
        numberOfProjectiles = nProjectiles;
        projectileToShoot = (GameObject)Resources.Load<GameObject>(PATH_TO_PROJECTILES + projectileName);
    }

    public void setCharacter(Transform character) {
        this.character = character;
    }

    public void castAbility()
    {
        if(character == null)
            setCharacter(GameObject.FindGameObjectWithTag("Player").transform);
        for (int i = 0; i < numberOfProjectiles; i++)
            UnityEngine.Object.Instantiate(projectileToShoot, character.position, character.rotation);
    }
}

