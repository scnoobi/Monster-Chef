using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

abstract class ProjectileMovement
{
    protected Transform shooter;
    protected Projectile projectile;

    public abstract void MoveProjectile();
}
