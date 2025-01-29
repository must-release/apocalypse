using UnityEngine;
using WeaponEnums;

public class Scratch : ShortRangeWeaponBase
{
    private Rigidbody2D rb;

    protected override void InitializeWeapon() 
    {
        base.InitializeWeapon();
        DamagePlayer = true;
        WeaponType = WEAPON_TYPE.SCRATCH;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        WeaponDamageInfo.damageValue = 1;
    }
}