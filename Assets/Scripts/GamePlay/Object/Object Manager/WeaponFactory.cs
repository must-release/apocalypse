using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using NUnit.Framework;

public class WeaponFactory : MonoBehaviour, IGamePlayInitializer
{
    public static WeaponFactory Instance { get; private set; }

    public bool IsInitialized { get; private set; }

    public IEnumerator AsyncPoolWeapons(GameObject owner, 
                                        WeaponType weaponType, 
                                        Queue<IWeapon> weapons, 
                                        int poolNum,
                                        bool attachToOwner = false
    )
    {
        yield return null;
        //         AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(AssetPath.WeaponAsset);
        //         yield return handle;
        //         if (handle.Status == AsyncOperationStatus.Succeeded)
        //         {
        //             // Copy Weapon 
        //             GameObject loadedWeapon = handle.Result;
        //             CreateAndEnqueueWeapon(loadedWeapon, owner, weapons, poolNum, attachToOwner);
        //         }
        //         else
        //         {
        //             Debug.LogError("Failed to load the weapon: " + WeaponAsset.GetWeaponPath(weaponType));
        //         }
    }

    public IEnumerator AsyncPoolAimingDots(WeaponType weaponType, List<AimingDot> aimingDots, Transform parentTransform)
    {
        Assert.IsTrue(_aimingDotPrefabs.ContainsKey(weaponType), $"{weaponType.ToString()} does not have aiming dots.");

        var poolingDot = _aimingDotPrefabs[weaponType];
        for (int i = 0; i < poolingDot.PoolCount; i++)
        {
            var handle = Addressables.InstantiateAsync(poolingDot.Prefab, parentTransform);
            yield return handle;

            if (AsyncOperationStatus.Failed == handle.Status)
            {
                Logger.Write(LogCategory.AssetLoad, $"Failed to instantiate {weaponType} aiming dot.");
                yield break;
            }

            GameObject dotCopy = handle.Result;
            dotCopy.name = $"{weaponType}_aimingDot_{i}";
            aimingDots.Add(dotCopy.GetComponent<AimingDot>());
            
            if (0 < i) 
                aimingDots[i - 1].NextDot = aimingDots[i];
        }
    }


    /****** Private Members ******/

    private struct PooledPrefabInfo
    {
        public GameObject Prefab;
        public int PoolCount;

        public PooledPrefabInfo(GameObject prefab, int poolCount)
        {
            Prefab = prefab;
            PoolCount = poolCount;
        }
    }

    private Dictionary<WeaponType, PooledPrefabInfo> _weaponPrefabs      = new();
    private Dictionary<WeaponType, PooledPrefabInfo> _aimingDotPrefabs   = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GamePlayManager.Instance.RegisterGamePlayInitializer(this);
        StartCoroutine(AsyncLoadWeaponAsset());
    }

    private IEnumerator AsyncLoadWeaponAsset()
    {
        AsyncOperationHandle<WeaponAsset> handle = Addressables.LoadAssetAsync<WeaponAsset>(AssetPath.WeaponAsset);
        yield return handle;

        if (AsyncOperationStatus.Failed == handle.Status)
        {
            Logger.Write(LogCategory.AssetLoad, "Failed to load weapon asset", LogLevel.Error);
        }

        var weaponAssets = handle.Result.WeaponAssets;
        foreach (var weaponEntry in weaponAssets)
        {
            _weaponPrefabs.Add(weaponEntry.WeaponType, new PooledPrefabInfo(weaponEntry.WeaponPrefab, weaponEntry.WeaponPoolCount));
            if (null != weaponEntry.AimingDotPrefab)
            {
                _aimingDotPrefabs.Add(weaponEntry.WeaponType, new PooledPrefabInfo(weaponEntry.AimingDotPrefab, weaponEntry.AimingDotPoolCount));
            }
        }

        IsInitialized = true;
    }

    private void CreateAndEnqueueWeapon(GameObject loadedWeapon,
                                        GameObject owner,
                                        Queue<IWeapon> weapons,
                                        int count,
                                        bool attachToOwner
)
    {
        for (int i = count; 0 < i; i--)
        {
            GameObject weaponInstance = (i == 1) ? loadedWeapon : Instantiate(loadedWeapon);
            IWeapon weapon = weaponInstance.GetComponent<IWeapon>();
            weapon.SetOwner(owner);
            weapons.Enqueue(weapon);
            weaponInstance.SetActive(false);

            if (attachToOwner)
                weaponInstance.transform.SetParent(owner.transform, false);
        }
    }
}
