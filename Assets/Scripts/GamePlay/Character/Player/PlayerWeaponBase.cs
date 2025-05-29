using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class PlayerWeaponBase : MonoBehaviour, IAsyncLoadObject
{
    /****** Public Members ******/

    public bool IsLoaded => _isLoaded;

    public abstract float       ReloadTime          { get; }
    public abstract WeaponType  PlayerWeaponType    { get; }

    public abstract void Aim(bool isAiming);

    public float Attack()
    {
        IWeapon weapon = _pooledWeapons.Dequeue();
        weapon.SetLocalPosition(_shootingPoint.position);
        weapon.Attack((_shootingPoint.position - _weaponPivot.position).normalized);
        _pooledWeapons.Enqueue(weapon);

        return weapon.PostDelay;
    }

    public void AimAttack(Vector2 direction)
    {
        IWeapon weapon = _pooledWeapons.Dequeue();
        weapon.SetLocalPosition(_shootingPoint.position);
        weapon.Attack(direction.normalized);
        _pooledWeapons.Enqueue(weapon);
    }

    public void RotateWeaponPivot(Vector3 target)
    {
        Vector3 direction = target - _weaponPivot.position;
        int flip = direction.x > 0 ? 0 : 180;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _weaponPivot.rotation = Quaternion.Euler(0, 0, flip + angle);
    }

    public void RotateWeaponPivot(float rotateAngle)
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
            Assert.IsTrue(0 < _pooledAimingDots.Count, $"Aiming Dots of {PlayerWeaponType} is not pooled.");
            return _pooledAimingDots;
        }
    }
    protected IWeapon CurrentWeapon
    {
        get
        {
            Assert.IsTrue(0 < _pooledWeapons.Count, $"{PlayerWeaponType} is not pooled");
            return _pooledWeapons.Peek();
        }
    }

    protected virtual IEnumerator Start()
    {
        yield return LoadWeaponsAndDots();
    }

    /****** Private Members ******/

    [SerializeField] private GameObject _playerObject   = null;
    [SerializeField] private Transform  _weaponPivot    = null;
    [SerializeField] private Transform  _shootingPoint  = null;

    private const int _WeaponPoolCount      = 15;
    private const int _AimingDotsPoolCount  = 20;

    private Queue<IWeapon>      _pooledWeapons      = new Queue<IWeapon>();
    private List<AimingDot>     _pooledAimingDots   = new List<AimingDot>();

    private bool _isLoaded = false;

    private void Awake()
    {
        Assert.IsTrue(_playerObject != null, "Player object is not assigned.");
        Assert.IsTrue(_weaponPivot != null, "Weapon pivot is not assigned.");
        Assert.IsTrue(_shootingPoint != null, "Shooting point is not assigned.");
    }

    private IEnumerator LoadWeaponsAndDots()
    {
        yield return WeaponFactory.Instance.AsyncPoolWeapons(_playerObject, PlayerWeaponType, _pooledWeapons, _WeaponPoolCount);
        yield return WeaponFactory.Instance.AsyncPoolAimingDots(PlayerWeaponType, _pooledAimingDots, _AimingDotsPoolCount);

        _isLoaded = true;
    }
}
