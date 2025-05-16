using UnityEngine;

public class Bullet : LongRangeWeaponBase
{
    /****** Public Members ******/

    public override WeaponType  WeaponType          => WeaponType.Bullet;
    public override bool        CanDamagePlayer     => false;
    public override float       FireSpeed           => 30f;
    public override float       ActiveDuration      => 5f;
    public override float       PostDelay           => 0f;

    public override void Attack(Vector3 direction)
    {
        base.Attack(direction);

        _bulletRigid.linearVelocity = direction * FireSpeed;
    }

    /****** Protected Members ******/

    protected override void Awake() 
    {
        base.Awake();

        _bulletRigid = GetComponent<Rigidbody2D>();
        _bulletRigid.gravityScale = _GravityScale;

        WeaponDamageInfo.damageValue = _Damage;
    }



    /****** Private Members ******/

    private const float _GravityScale   = 0;
    private const int   _Damage         = 1;

    private Rigidbody2D _bulletRigid = null;
}