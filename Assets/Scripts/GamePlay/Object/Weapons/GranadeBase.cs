using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class GranadeBase : LongRangeWeaponBase
{
    /****** Public Members ******/

    public abstract EffectType GranadeEffect { get; }

    public override bool CanDamagePlayer    => false;
    public override float FireSpeed         => 15f;
    public override float GravityScale      => _GravityScale;
    public override float ActiveDuration    => 1f;
    public override float PostDelay         => 0.5f;

    public override void Attack(Vector3 direction)
    {
        base.Attack(direction);

        _granadeRigid.linearVelocity    = direction * FireSpeed;
        _granadeRigid.angularVelocity   = _AngleSpeed;
    }

    public override void SetOwner(GameObject owner)
    {
        base.SetOwner(owner);

        _ownerCollider = owner.GetComponent<Collider2D>();
    }


    /****** Protected Members ******/

    protected override void Awake()
    {
        base.Awake();

        _granadeRigid       = GetComponent<Rigidbody2D>();
        _granadeCollider    = GetComponent<Collider2D>();

        _granadeRigid.gravityScale      = _GravityScale;
        WeaponDamageInfo.damageValue    = _Damage;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _granadeRigid.angularDamping = _InitialAngularDrag;

        if (_ownerCollider)
            Physics2D.IgnoreCollision(_ownerCollider, _granadeCollider);

    }

    protected virtual void Explode()
    {
        if (null != _countDownCoroutine)
        {
            StopCoroutine(_countDownCoroutine);
            _countDownCoroutine = null;
        }

        ShowExplosionEffect();
        ActiveDamageArea();

        gameObject.SetActive(false);
    }


    /****** Private Members ******/

    private const float _GravityScale           = 2f;
    private const float _AngleSpeed             = 240;
    private const float _InitialAngularDrag     = 0.05f;
    private const float _AngularDragAfterHit    = 3f;
    private const float _CountDownTime          = 0.5f;
    private const int   _Damage                 = 1;
    private const float _DamageRadius           = 3f;

    private Rigidbody2D _granadeRigid       = null;
    private Collider2D  _granadeCollider    = null;
    private Collider2D  _ownerCollider      = null;
    private Coroutine   _countDownCoroutine = null;

    private IEnumerator StartCountDown()
    {
        yield return new WaitForSeconds(_CountDownTime);

        Explode();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _granadeRigid.angularDamping = _AngularDragAfterHit;

        if (null == _countDownCoroutine)
            _countDownCoroutine = StartCoroutine(StartCountDown());

        if (collision.transform.TryGetComponent(out CharacterBase character))
        {
            if (false == character.CompareTag("Player"))
            {
                Explode();
            }
        }
    }

    private void ShowExplosionEffect()
    {
        IEffect explosionEffect = PoolManager.Instance.Get<EffectType, IEffect>(EffectType.NormalExplosion);
        explosionEffect.SetPosition(transform.position);
        explosionEffect.Play();
        explosionEffect.OnEffectComplete += () =>
        {
            PoolManager.Instance.Return(EffectType.NormalExplosion, explosionEffect);
        };
    }

    private void ActiveDamageArea()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _DamageRadius, 1 << LayerMask.NameToLayer(Layer.Character));
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<ICharacter>();
            if (false == enemy.IsPlayer)
            {
                enemy.OnDamaged(WeaponDamageInfo);
            }
        }
    }
}
