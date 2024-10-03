using UnityEngine;
using WeaponEnums;

public class Bullet : WeaponBase
{
    private Rigidbody2D rb;
    private float fireSpeed;

    private void Awake() 
    {
        DamagePlayer = false;
        WeaponType = WEAPON_TYPE.BULLET;

        rb = GetComponent<Rigidbody2D>();
        fireSpeed = 12;
    }

    public override void Fire(Vector2 direction)
    {
        base.Fire(direction);
        rb.velocity = direction * fireSpeed;
    }
}