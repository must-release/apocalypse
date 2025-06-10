using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections;
using UnityEngine;

public class HeroWeapon : PlayerWeaponBase
{
    /****** Public Members ******/

    public override float ReloadTime => 0.5f;
    public override ProjectileType PlayerWeaponType => ProjectileType.Bullet;

    public override void Aim(bool isAiming)
    {
        if (false == isAiming)
        {
            AimingDotList.ForEach((dot) => dot.gameObject.SetActive(false));
            return;
        }

        Vector3 start = ShootingPointPosition;
        Vector3 direction = (ShootingPointPosition - WeaponPivotPosition).normalized;
        Vector3 velocity = direction * _fireSpeed;

        float timeStep = 0.03f;

        for (int i = 1; i < AimingDotList.Count; i++)
        {
            var dot = AimingDotList[i];
            dot.gameObject.SetActive(true);

            float t = timeStep * i;

            Vector3 position = start + velocity * t;
            dot.transform.position = position;
            dot.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
        }
    }

    /****** Protected Members ******/

    // TODO: Must not get example weapon from PoolHandler
    protected override void SetWeaponInfo()
    {
        var heroWeapon = WeaponPool.Get();
        _fireSpeed = heroWeapon.FireSpeed;

        WeaponPool.Return(heroWeapon);
    }

    /****** Private Members ******/

    private float _fireSpeed;
}
