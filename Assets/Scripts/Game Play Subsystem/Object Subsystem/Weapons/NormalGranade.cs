using System.Collections;
using LayerEnums;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class NormalGranade : LongRangeWeaponBase
{
    /****** Public Members ******/

    public override WeaponType  WeaponType          => WeaponType.NormalGranade;
    public override bool        CanDamagePlayer     => false;
    public override float       FireSpeed           => 30f;
    public override float       ActiveDuration      => 1f;
    public override float       PostDelay           => 1f;

    public override void Attack(Vector3 direction)
    {
        base.Attack(direction);

        _granadeRigid.velocity          = direction * FireSpeed;
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

        _granadeRigid.angularDrag = _initialAngularDrag;

        if (_ownerCollider)
            Physics2D.IgnoreCollision(_ownerCollider, _granadeCollider);

    }


    /****** Private Members ******/

    private const float _GravityScale           = 5f;
    private const float _AngleSpeed             = 240;
    private const float _initialAngularDrag     = 0.05f;
    private const float _AngularDragAfterHit    = 3f;
    private const float _countDownTime          = 2f;    
    private const int   _Damage                 = 1;

    private Rigidbody2D _granadeRigid       = null;
    private Collider2D  _granadeCollider    = null;
    private Collider2D  _ownerCollider      = null;
    private Coroutine   _countDownCoroutine = null;

    private void Explode()
    {
        if (null != _countDownCoroutine)
        {
            StopCoroutine(_countDownCoroutine);
            _countDownCoroutine = null;
        }

        Debug.Log("Explode");

        gameObject.SetActive(false);
    }

    private IEnumerator StartCountDown()
    {
        yield return new WaitForSeconds(_countDownTime);

        Explode();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _granadeRigid.angularDrag = _AngularDragAfterHit;

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
}
