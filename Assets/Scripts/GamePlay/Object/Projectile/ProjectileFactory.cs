using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using Cysharp.Threading.Tasks;

public class ProjectileFactory : MonoBehaviour, IGamePlayInitializer
{
    public static ProjectileFactory Instance { get; private set; }

    public bool IsInitialized { get; private set; }

    public async UniTask<ProjectilePoolHandler> AsyncLoadPoolHandler(ProjectileType projectileType)
    {
        if (false == _poolHandlers.ContainsKey(projectileType))
        {
            await PoolManager.Instance.AsyncRegisterPool<ProjectileType, IProjectile>(projectileType,
                _poolObjectInfo[projectileType].ObjectReference, _poolObjectInfo[projectileType].PoolCount, $"ProjectilePool/{projectileType}").ToUniTask();

            _poolHandlers.Add(projectileType, new ProjectilePoolHandler(projectileType));
        }

        return _poolHandlers[projectileType];
    }

    public async UniTask AsyncPoolAimingDots(ProjectileType projectileType, List<AimingDot> aimingDots, Transform parentTransform)
    {
        Debug.Assert(_aimingDotPoolObjectInfo.ContainsKey(projectileType), $"{projectileType} does not have aiming dots.");

        var poolingDot = _aimingDotPoolObjectInfo[projectileType];
        for (int i = 0; i < poolingDot.PoolCount; i++)
        {
            var handle = Addressables.InstantiateAsync(poolingDot.ObjectReference, parentTransform);
            await handle.ToUniTask();

            if (AsyncOperationStatus.Failed == handle.Status)
            {
                Logger.Write(LogCategory.AssetLoad, $"Failed to instantiate {projectileType} aiming dot.");
                return;
            }

            GameObject dotCopy = handle.Result;
            dotCopy.name = $"{projectileType}_aimingDot_{i}";
            dotCopy.gameObject.SetActive(false);
            aimingDots.Add(dotCopy.GetComponent<AimingDot>());

            if (0 < i)
            {
                aimingDots[i - 1].NextDot = aimingDots[i];
            }
        }
    }


    /****** Private Members ******/

    private struct PoolObjectInfo
    {
        public AssetReferenceGameObject ObjectReference { get; }
        public int PoolCount { get; }

        public PoolObjectInfo(AssetReferenceGameObject objectReference, int poolCount)
        {
            ObjectReference = objectReference;
            PoolCount       = poolCount;
        }
    }

    private Dictionary<ProjectileType, PoolObjectInfo>          _poolObjectInfo       = new();
    private Dictionary<ProjectileType, PoolObjectInfo>          _aimingDotPoolObjectInfo    = new();
    private Dictionary<ProjectileType, ProjectilePoolHandler>   _poolHandlers         = new();

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
        AD.GamePlay.GamePlayManager.Instance.RegisterGamePlayInitializer(this);
        StartCoroutine(AsyncLoadProjectileAsset());
    }

    private IEnumerator AsyncLoadProjectileAsset()
    {
        AsyncOperationHandle<ProjectileAsset> handle = Addressables.LoadAssetAsync<ProjectileAsset>(AssetPath.ProjectileAsset);
        yield return handle;

        if (AsyncOperationStatus.Failed == handle.Status)
        {
            Logger.Write(LogCategory.AssetLoad, "Failed to load weapon asset", LogLevel.Error);
        }

        var projectileAssets = handle.Result.ProjectileAssets;
        foreach (var projectileEntry in projectileAssets)
        {
            _poolObjectInfo.Add(projectileEntry.SelectedProjectileType, new PoolObjectInfo(projectileEntry.ProjectileReference, projectileEntry.ProjectilePoolCount));
            if (null != projectileEntry.AimingDotReference)
            {
                _aimingDotPoolObjectInfo.Add(projectileEntry.SelectedProjectileType, new PoolObjectInfo(projectileEntry.AimingDotReference, projectileEntry.AimingDotPoolCount));
            }
        }

        IsInitialized = true;
    }
}
