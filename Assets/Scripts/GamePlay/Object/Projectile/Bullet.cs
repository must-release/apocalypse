using UnityEngine;

public class Bullet : ProjectileBase
{
    /****** Public Members ******/

    public override ProjectileType  CurrentPojectileType    => ProjectileType.Bullet;
    public override bool            CanDamagePlayer         => false;
    public override float           FireSpeed               => 15f;
    public override float           GravityScale            => _GravityScale;
    public override float           FireDuration            => 5f;
    public override float           PostFireDelay           => 0.5f;

    public override void Fire(Vector3 direction)
    {
        base.Fire(direction);

        _bulletRigid.linearVelocity = direction * FireSpeed;
    }

    /****** Protected Members ******/

    protected override void Awake() 
    {
        base.Awake();

        _bulletRigid = GetComponent<Rigidbody2D>();
        _bulletRigid.gravityScale = _GravityScale;

        ProjectileDamageInfo.DamageValue = _Damage;
    }



    /****** Private Members ******/

    private const float _GravityScale   = 0;
    private const int   _Damage         = 1;

    private Rigidbody2D _bulletRigid = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out CharacterBase character))
        {
            if (false == character.IsPlayer)
            {
                character.OnDamaged(ProjectileDamageInfo);
                ExpireProjectile();
            }
            
            return;
        }

        if (false == collision.isTrigger)
        {
            ExpireProjectile();
        }
    }
}