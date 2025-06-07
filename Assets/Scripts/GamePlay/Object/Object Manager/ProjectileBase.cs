using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider2D))]
public abstract class ProjectileBase : MonoBehaviour, IProjectile
{
    public DamageInfo ProjectileDamageInfo { get; protected set; } = new DamageInfo();

    public abstract ProjectileType  CurrentPojectileType    { get; }
    public abstract bool            CanDamagePlayer         { get; }
    public abstract float           FireDuration            { get; }
    public abstract float           PostFireDelay           { get; }
    public abstract float           FireSpeed               { get; }
    public abstract float           GravityScale            { get; }

    public event Action OnProjectileExpired;

    public virtual void Fire(Vector3 direction)
    {
        Assert.IsTrue(null != ProjectileDamageInfo.Attacker, $"Onwer of the {CurrentPojectileType} is not set.");
        Assert.IsTrue(_isPositionSet, $"Position of the {CurrentPojectileType} is not set.");

        _isFired = true;
        transform.localRotation = Quaternion.FromToRotation(Vector3.right, direction);
    }

    public virtual void SetOwner(GameObject owner)
    {
        Assert.IsTrue(null != owner, "Owner of the weapon is not assigned");
        Assert.IsTrue(null == ProjectileDamageInfo.Attacker, $"Owner of the {CurrentPojectileType} is already set.");

        ProjectileDamageInfo.Attacker = owner;
    }

    public void SetProjectilePosition(Vector3 position)
    {
        _isPositionSet      = true;
        transform.position  = position;
    }

    public virtual void OnGetFromPool()
    {
        OnProjectileExpired             = null;
        ProjectileDamageInfo.Attacker   = null;
        transform.position              = Vector3.zero;
        _isFired                        = false;   
        _isPositionSet                  = false;

        gameObject.SetActive(true);
    }

    public virtual void OnReturnToPool() 
    {
        if (null != _offScreenCheckCoroutine)
        {
            StopCoroutine(_offScreenCheckCoroutine);
            _offScreenCheckCoroutine = null;
        }

        gameObject.SetActive(false);
    }


    /****** Protected Methods ******/

    protected virtual void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer(Layer.Projectile);
    }

    protected void ExpireProjectile()
    {
        Assert.IsTrue(null != OnProjectileExpired, $"OnProjectileExpired is null in {CurrentPojectileType}.");

        OnProjectileExpired.Invoke();
    }

    /****** Private Members ******/

    private const float _MaxOffscreenTime = 10f;

    private Coroutine   _offScreenCheckCoroutine;
    private bool        _isFired;
    private bool        _isPositionSet;

    private void Update() 
    {
        if (false == _isFired) return;

        CheckOffScreen(); 
    }

    private void CheckOffScreen()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        if (0 < viewportPos.x && viewportPos.x < 1 && 0 < viewportPos.y && viewportPos.y < 1)
        {
            if (null != _offScreenCheckCoroutine)
            {
                StopCoroutine(_offScreenCheckCoroutine);
                _offScreenCheckCoroutine = null;
            }
        }
        else
        {
            _offScreenCheckCoroutine = StartCoroutine(AsynExpireProjectile());
        }
    }

    private IEnumerator AsynExpireProjectile()
    {
        yield return new WaitForSeconds(_MaxOffscreenTime);

        OnProjectileExpired();
    }
}
