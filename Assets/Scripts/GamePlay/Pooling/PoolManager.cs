using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine;
using UnityEngine.AddressableAssets;

[DisallowMultipleComponent]
public class PoolManager : MonoBehaviour, IGamePlayInitializer
{
    /****** Public Members ******/

    public static PoolManager Instance { get; private set; }

    public bool IsInitialized { get; private set; }

    public IEnumerator AsyncRegisterPool<TEnum, TPoolObject>(TEnum id, AssetReferenceGameObject prefab, int preloadCount, string poolContainerPath)
        where TEnum : struct, Enum
        where TPoolObject : IPoolable
    {
        var pools = PoolHolder<TEnum, TPoolObject>.Pools;
        Debug.Assert(false == pools.ContainsKey(id), $"Pool for enum '{id}' is already registered.");


        if (false == _poolConainterObjects.ContainsKey(poolContainerPath))
        {
            var containerGO = CreateOrGetContainerHierarchy(poolContainerPath, transform);
            _poolConainterObjects.Add(poolContainerPath, containerGO);
        }

        var newPool = new ObjectPool<TPoolObject>(prefab, _poolConainterObjects[poolContainerPath].transform);
        yield return newPool.Preload(preloadCount);

        pools.Add(id, newPool);
    }

    public TPoolObject Get<TEnum, TPoolObject>(TEnum id)
        where TEnum : struct, Enum
        where TPoolObject : IPoolable
    {
        return PoolHolder<TEnum, TPoolObject>.Get(id);
    }

    public void Return<TEnum, TPoolObject>(TEnum id, TPoolObject obj)
        where TEnum : struct, Enum
        where TPoolObject : IPoolable
    {
        PoolHolder<TEnum, TPoolObject>.Return(id, obj);
    }

    /****** Private Members ******/

    private Dictionary<string, GameObject> _poolConainterObjects = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning($"{nameof(PoolManager)} already exists. Destroying duplicate.");
            Destroy(this);
        }
    }

    private void Start()
    {
        GamePlayManager.Instance.RegisterGamePlayInitializer(this);
        StartCoroutine(AsyncInitializePool());
    }

    private static class PoolHolder<TEnum, TPoolObject>
        where TEnum : struct, Enum
        where TPoolObject : IPoolable
    {
        internal static readonly Dictionary<TEnum, ObjectPool<TPoolObject>> Pools = new();

        internal static TPoolObject Get(TEnum id)
        {
            Debug.Assert(Pools.ContainsKey(id), $"No pool registered for ID '{id}'. Did you call RegisterPool?");

            return Pools[id].Get();
        }

        internal static void Return(TEnum id, TPoolObject obj)
        {
            Debug.Assert(Pools.ContainsKey(id), $"No pool registered for ID '{id}'. Did you call RegisterPool?");

            Pools[id].Return(obj);
        }
    }

    private GameObject CreateOrGetContainerHierarchy(string path, Transform root)
    {
        string[] parts = path.Split('/');
        Transform current = root;

        foreach (string part in parts)
        {
            Transform child = current.Find(part);
            if (child == null)
            {
                GameObject newGO = new GameObject(part);
                newGO.transform.SetParent(current, false);
                child = newGO.transform;
            }
            current = child;
        }

        return current.gameObject;
    }

    private IEnumerator AsyncInitializePool()
    {
        yield return PoolInitializer.AsyncLoadPool();

        IsInitialized = true;
    }
}