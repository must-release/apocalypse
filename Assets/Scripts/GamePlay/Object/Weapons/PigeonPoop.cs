using UnityEngine;

public class PigeonPoop : ProjectileBase
{
    /****** Public Members ******/

    public override ProjectileType  CurrentPojectileType          => ProjectileType.PigeonPoop;
    public override bool        CanDamagePlayer     => true;
    public override float       FireSpeed           => 10f;
    public override float       GravityScale        => _GravityScale;
    public override float       FireDuration      => 1f;
    public override float       PostFireDelay           => 1f;

    public override void Fire(Vector3 direction)
    {
        base.Fire(direction);

        _poopRigid.linearVelocity = direction * FireSpeed;
    }

    /****** Protected Members ******/

    protected override void Awake() 
    {
        base.Awake();

        _poopRigid = GetComponent<Rigidbody2D>();
        _poopRigid.gravityScale = _GravityScale;

        ProjectileDamageInfo.DamageValue = _Damage;
    }



    /****** Private Members ******/

    private const float _GravityScale   = 0;
    private const int   _Damage         = 1;

    private Rigidbody2D _poopRigid;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out CharacterBase character))
        {
            if (character.IsPlayer)
            {
                character.OnDamaged(ProjectileDamageInfo);
                return;
            }
        }

        if (false == collision.isTrigger)
        {
            ExpireProjectile();
        }
    }
}