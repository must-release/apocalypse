using UnityEngine;

public class PigeonPoop : LongRangeWeaponBase
{
    /****** Public Members ******/

    public override WeaponType  WeaponType          => WeaponType.PigeonPoop;
    public override bool        CanDamagePlayer     => true;
    public override float       FireSpeed           => 30f;
    public override float       GravityScale        => _GravityScale;
    public override float       ActiveDuration      => 1f;
    public override float       PostDelay           => 1f;

    public override void Attack(Vector3 direction)
    {
        base.Attack(direction);

        _poopRigid.linearVelocity = direction * FireSpeed;
    }

    /****** Protected Members ******/

    protected override void Awake() 
    {
        base.Awake();

        _poopRigid = GetComponent<Rigidbody2D>();
        _poopRigid.gravityScale = _GravityScale;

        WeaponDamageInfo.damageValue = _Damage;
    }



    /****** Private Members ******/

    private const float _GravityScale   = 0;
    private const int   _Damage         = 1;

    private Rigidbody2D _poopRigid = null;
}