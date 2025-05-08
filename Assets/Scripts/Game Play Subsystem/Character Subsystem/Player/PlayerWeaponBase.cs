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
    public abstract void Attack();

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

    private void Start() 
    { 
        StartCoroutine(LoadWeaponsAndDots());
    }

    private IEnumerator LoadWeaponsAndDots()
    {
        bool isWeaponLoaded = false;
        bool isAimingDotsLoaded = false;

        StartCoroutine(LoadWeapons(() => isWeaponLoaded = true));
        StartCoroutine(LoadAimingDots(() => isAimingDotsLoaded = true));

        yield return new WaitUntil(() => isWeaponLoaded && isAimingDotsLoaded);
        _isLoaded = true;
    }

    private IEnumerator LoadWeapons(System.Action action)
    {
        yield return StartCoroutine(WeaponFactory.Instance.AsyncPoolWeapons(_playerObject, PlayerWeaponType, _pooledWeapons, _WeaponPoolCount));

        action.Invoke();
    }

    private IEnumerator LoadAimingDots(System.Action action)
    {
        yield return StartCoroutine(WeaponFactory.Instance.AsyncPoolAimingDots(PlayerWeaponType, _pooledAimingDots, _AimingDotsPoolCount));

        action.Invoke();
    }
}
