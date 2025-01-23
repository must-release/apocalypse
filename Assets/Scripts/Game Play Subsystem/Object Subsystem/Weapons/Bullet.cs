using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using WeaponEnums;

public class Bullet : WeaponBase
{
    private Rigidbody2D rb;

    protected override void InitializeWeapon() 
    {
        base.InitializeWeapon();
        DamagePlayer = false;
        WeaponType = WEAPON_TYPE.BULLET;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        FireSpeed = 30;
        WeaponDamageInfo.damageValue = 1;
    }

    public override void Fire(Vector3 direction)
    {
        base.Fire(direction);
        rb.velocity = direction * FireSpeed;
    }
}