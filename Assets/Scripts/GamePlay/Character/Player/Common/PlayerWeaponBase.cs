using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class PlayerWeaponBase : MonoBehaviour, IAsyncLoadObject
{
    /****** Public Members ******/

    public bool IsLoaded => _isLoaded;

    public abstract float       ReloadTime          { get; }
    public abstract ProjectileType  PlayerWeaponType    { get; }

    public abstract void Aim(bool isAiming);

    public float Attack()
    {
        IProjectile weapon = WeaponPool.Get();
        weapon.OnProjectileExpired += () => WeaponPool.Return(weapon);
        weapon.SetOwner(_playerObject);
        weapon.SetProjectilePosition(_shootingPoint.position);
        weapon.Fire((_shootingPoint.position - _weaponPivot.position).normalized);

        return weapon.PostFireDelay;
    }

    public void AimAttack(Vector2 direction)
    {
        IProjectile weapon = _pooledWeapons.Dequeue();
        weapon.SetProjectilePosition(_shootingPoint.position);
        weapon.Fire(direction.normalized);
        _pooledWeapons.Enqueue(weapon);
    }

    public void RotateWeaponPivot(Vector3 target)
    {
        Vector3 direction = target - _weaponPivot.position;
        int flip = direction.x > 0 ? 0 : 180;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _weaponPivot.rotation = Quaternion.Euler(0, 0, flip + angle);
    }

    public void SetWeaponPivotRotation(float rotateAngle)
    {
        _weaponPivot.localRotation = Quaternion.Euler(0, 0, rotateAngle);
    }

    /****** Protected Members ******/

    protected Vector2 WeaponPivotPosition   => _weaponPivot.position;
    protected Vector2 ShootingPointPosition => _shootingPoint.position;
    protected List<AimingDot> AimingDotList
    {
        get
        {
            Debug.Assert(0 < _pooledAimingDots.Count, $"Aiming Dots of {PlayerWeaponType} is not pooled.");
            return _pooledAimingDots;
        }
    }
    protected ProjectilePoolHandler WeaponPool
    {
        get
        {
            Debug.Assert(null != _weaponPoolHandler, $"{PlayerWeaponType} Weapon is not pooled.");
            return _weaponPoolHandler;
        }
    }

    protected virtual void Start()
    {
        LoadWeaponsAndDots().Forget();
    }

    protected abstract void SetWeaponInfo();

    /****** Private Members ******/

    [SerializeField] private GameObject _playerObject   = null;
    [SerializeField] private Transform  _weaponPivot    = null;
    [SerializeField] private Transform  _shootingPoint  = null;

    private Queue<IProjectile>      _pooledWeapons      = new Queue<IProjectile>();
    private List<AimingDot>         _pooledAimingDots   = new List<AimingDot>();
    private ProjectilePoolHandler   _weaponPoolHandler;

    private Transform _aimingDotsTransform;

    private bool _isLoaded;

    private void Awake()
    {
        Debug.Assert(_playerObject != null, "Player object is not assigned.");
        Debug.Assert(_weaponPivot != null, "Weapon pivot is not assigned.");
        Debug.Assert(_shootingPoint != null, "Shooting point is not assigned.");

        _aimingDotsTransform = new GameObject("PooledAimingDots").transform;
        _aimingDotsTransform.SetParent(transform, false);
    }

    private async UniTask LoadWeaponsAndDots()
    {
        _weaponPoolHandler = await ProjectileFactory.Instance.AsyncLoadPoolHandler(PlayerWeaponType);
        await ProjectileFactory.Instance.AsyncPoolAimingDots(PlayerWeaponType,  _pooledAimingDots, _aimingDotsTransform);

        SetWeaponInfo();

        _isLoaded = true;
    }
}
