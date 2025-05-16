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

    public abstract void Aim(Vector3 direction);

    public void Attack()
    {
        IWeapon weapon = _pooledWeapons.Dequeue();
        weapon.SetLocalPosition(_shootingPoint.position);
        weapon.Attack((_shootingPoint.position - _weaponPivot.position).normalized);
        _pooledWeapons.Enqueue(weapon);
    }

    /****** Protected Members ******/



    /****** Private Members ******/

    [SerializeField] private GameObject _playerObject   = null;
    [SerializeField] private Transform  _weaponPivot    = null;
    [SerializeField] private Transform  _shootingPoint  = null;

    private const int _WeaponPoolCount      = 15;
    private const int _AimingDotsPoolCount  = 40;

    private Queue<IWeapon>      _pooledWeapons      = new Queue<IWeapon>();
    private List<GameObject>    _pooledAimingDots   = new List<GameObject>();

    private bool _isLoaded = false;

    private void Awake()
    {
        Assert.IsTrue(_playerObject != null, "Player object is not assigned.");
        Assert.IsTrue(_weaponPivot != null, "Weapon pivot is not assigned.");
        Assert.IsTrue(_shootingPoint != null, "Shooting point is not assigned.");
    }

    private IEnumerator Start() 
    { 
        yield return LoadWeaponsAndDots();
    }

    private IEnumerator LoadWeaponsAndDots()
    {
        yield return WeaponFactory.Instance.AsyncPoolWeapons(_playerObject, PlayerWeaponType, _pooledWeapons, _WeaponPoolCount);
        yield return WeaponFactory.Instance.AsyncPoolAimingDots(PlayerWeaponType, _pooledAimingDots, _AimingDotsPoolCount);

        _isLoaded = true;
    }
}
