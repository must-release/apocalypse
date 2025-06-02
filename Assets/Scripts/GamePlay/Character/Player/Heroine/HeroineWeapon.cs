using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections;
using UnityEngine;

public class HeroineWeapon : PlayerWeaponBase
{
    /****** Public Members ******/

    public override float       ReloadTime          => 0.5f;
    public override WeaponType  PlayerWeaponType    => WeaponType.NormalGranade;

    public override void Aim(bool isAiming)
    {
        if (false == isAiming)
        {
            AimingDotList.ForEach((dot) => dot.gameObject.SetActive(false));
            return;
        }

        Vector3 start       = ShootingPointPosition;
        Vector3 direction   = (ShootingPointPosition - WeaponPivotPosition).normalized;
        Vector3 velocity    = direction * _fireSpeed;
        Vector3 gravity     = Physics.gravity * _gravityScale;

        float timeStep = 0.05f;

        for (int i = 1; i < AimingDotList.Count; i++)
        {
            var dot = AimingDotList[i];
            dot.gameObject.SetActive(true);

            float t = timeStep * i;

            Vector3 position = start + velocity * t + 0.5f * gravity * t * t;
            dot.transform.position = position;
        }
    }

    /****** Protected Members ******/

    protected override IEnumerator Start()
    {
        yield return base.Start();

        Assert.IsTrue(CurrentWeapon is LongRangeWeaponBase, "Weapon of the Heroine must be a long range weapon");

        var heroineWeapon   = CurrentWeapon as LongRangeWeaponBase;
        _fireSpeed          = heroineWeapon.FireSpeed;
        _gravityScale       = heroineWeapon.GravityScale;
    }


    /****** Private Members ******/

    private float _fireSpeed;
    private float _gravityScale;
}
