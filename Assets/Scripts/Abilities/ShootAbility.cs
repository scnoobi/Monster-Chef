using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

   public class ShootAbility : Ability
    {
        public int numberOfProjectiles;

        GameObject projectileToShoot;
        Transform character;

        public ShootAbility(int numberOfProjectiles) {
            this.numberOfProjectiles = numberOfProjectiles;
        }

        public void setCharacter(Transform character) {
            this.character = character;
        }

        public void castAbility()
        {
            GameObject.Instantiate(projectileToShoot);
        }
    }

