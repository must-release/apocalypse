using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using Cysharp.Threading.Tasks;

public class WeaponFactory : MonoBehaviour, IGamePlayInitializer
{
    public static WeaponFactory Instance { get; private set; }

    public bool IsInitialized { get; private set; }

    public async UniTask<WeaponPoolHandler> AsyncLoadWeaponPoolHandler(WeaponType weaponType)
    {
        if (false == _weaponPoolHandlers.ContainsKey(weaponType))
        {
            await PoolManager.Instance.AsyncRegisterPool<WeaponType, IWeapon>(weaponType,
                _weaponPoolObjectInfo[weaponType].ObjectReference, _weaponPoolObjectInfo[weaponType].PoolCount, $"WeaponPool/{weaponType}").ToUniTask();

            _weaponPoolHandlers.Add(weaponType, new WeaponPoolHandler(weaponType));
        }

        return _weaponPoolHandlers[weaponType];
    }

    public async UniTask AsyncPoolAimingDots(WeaponType weaponType, List<AimingDot> aimingDots, Transform parentTransform)
    {
        Assert.IsTrue(_aimingDotPoolObjectInfo.ContainsKey(weaponType), $"{weaponType} does not have aiming dots.");

        var poolingDot = _aimingDotPoolObjectInfo[weaponType];
        for (int i = 0; i < poolingDot.PoolCount; i++)
        {
            var handle = Addressables.InstantiateAsync(poolingDot.ObjectReference, parentTransform);
            await handle.ToUniTask();

            if (AsyncOperationStatus.Failed == handle.Status)
            {
                Logger.Write(LogCategory.AssetLoad, $"Failed to instantiate {weaponType} aiming dot.");
                return;
            }

            GameObject dotCopy = handle.Result;
            dotCopy.name = $"{weaponType}_aimingDot_{i}";
            aimingDots.Add(dotCopy.GetComponent<AimingDot>());
            
            if (0 < i) 
                aimingDots[i - 1].NextDot = aimingDots[i];
        }
    }


    /****** Private Members ******/

    private struct PoolObjectInfo
    {
        public AssetReferenceGameObject ObjectReference;
        public int PoolCount;

        public PoolObjectInfo(AssetReferenceGameObject objectReference, int poolCount)
        {
            ObjectReference = objectReference;
            PoolCount = poolCount;
        }
    }

    private Dictionary<WeaponType, PoolObjectInfo>      _weaponPoolObjectInfo       = new();
    private Dictionary<WeaponType, PoolObjectInfo>      _aimingDotPoolObjectInfo    = new();
    private Dictionary<WeaponType, WeaponPoolHandler>   _weaponPoolHandlers         = new();

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
            _weaponPoolObjectInfo.Add(weaponEntry.WeaponType, new PoolObjectInfo(weaponEntry.WeaponReference, weaponEntry.WeaponPoolCount));
            if (null != weaponEntry.AimingDotReference)
            {
                _aimingDotPoolObjectInfo.Add(weaponEntry.WeaponType, new PoolObjectInfo(weaponEntry.AimingDotReference, weaponEntry.AimingDotPoolCount));
            }
        }

        IsInitialized = true;
    }
}
