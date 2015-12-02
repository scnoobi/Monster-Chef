using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Bullet : ProjectileMovement
{
    Vector3 originPos;
    public Bullet(Transform shooter, Projectile projectile)
    {
        this.shooter = shooter;
        originPos = shooter.position;
        this.projectile = projectile;
    }

    public override void MoveProjectile()
    {
        if (shooter.position.y == originPos.y)
        {
            projectile.Activate();
        }
    }
}

