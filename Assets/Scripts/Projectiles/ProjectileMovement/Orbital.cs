using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Orbital : ProjectileMovement
{
    float radius = 3f;
    float radiusSpeed = .5f;
    float rotationSpeed = .7f;
    float rotationAngle = .7f;
    float currentLerpTime = 0;
    float lerpTime = 1f;

    public Orbital(Transform shooter, Projectile projectile)
    {
        this.shooter = shooter;
        this.projectile = projectile;
    }

    public override void MoveProjectile()
    {

        currentLerpTime = Mathf.Clamp(currentLerpTime + Time.deltaTime, 0, 1);
        float perc = currentLerpTime / 1;
        if(perc == 1 && rotationSpeed == .7f)
        {
            rotationSpeed *= 2;
        }

        rotationAngle += 0.7f * Time.deltaTime * rotationSpeed;

        float originX = shooter.transform.position.x;
        float originY = shooter.transform.position.y;
        float realRadius = Mathf.Lerp(0, radius, radiusSpeed * perc);
        float x = originX + realRadius * Mathf.Cos(rotationAngle);
        float y = originY + realRadius * Mathf.Sin(rotationAngle);
        projectile.transform.position = new Vector3(x, y);
    }
}

