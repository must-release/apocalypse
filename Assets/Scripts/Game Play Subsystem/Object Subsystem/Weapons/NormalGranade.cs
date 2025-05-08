using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGranade : LongRangeWeaponBase
{
    /****** Public Members ******/

    public override WeaponType  WeaponType      => WeaponType.NormalGranade;
    public override bool        CanDamagePlayer => false;
    public override float       FireSpeed       => 5f;
    public override float       ActiveDuration  => 1f;
    public override float       PostDelay       => 1f;

    public override void Attack(Vector3 direction)
    {
        base.Attack(direction);

        _granadeRigid.velocity = direction * FireSpeed;
    }

    /****** Protected Members ******/

    protected override void Start()
    {
        base.Start();

        _granadeRigid = GetComponent<Rigidbody2D>();
        _granadeRigid.gravityScale = _GravityScale;

        WeaponDamageInfo.damageValue = _Damage;
    }



    /****** Private Members ******/

    private const float _GravityScale   = 3f;
    private const int   _Damage         = 1;

    private Rigidbody2D _granadeRigid   = null;
}
