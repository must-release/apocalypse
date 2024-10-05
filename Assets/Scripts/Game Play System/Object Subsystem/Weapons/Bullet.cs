using UnityEngine;
using WeaponEnums;

public class Bullet : WeaponBase
{
    private Rigidbody2D rb;
    private float fireSpeed;

    protected override void InitializeWeapon() 
    {
        base.InitializeWeapon();
        DamagePlayer = false;
        WeaponType = WEAPON_TYPE.BULLET;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        fireSpeed = 30;
    }

    public override void Fire(Vector3 direction)
    {
        base.Fire(direction);
        rb.velocity = direction.normalized * fireSpeed;
    }

    public override void Aim(Vector3 startPos, Vector3 direction)
    {

    }
}