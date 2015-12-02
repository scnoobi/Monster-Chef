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
    private const string PATH_TO_PROJECTILES = "GameObject/Projectiles/";
    PatternManager patternManager;

    #region constructors
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
    #endregion

    public void castAbility(Transform caster)
    {
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            GameObject bullet = (GameObject)UnityEngine.Object.Instantiate(projectileToShoot, caster.position, caster.rotation);
            bullet.GetComponent<Projectile>().shooter = caster;
        }
    }
}

