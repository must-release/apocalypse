using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

[DisallowMultipleComponent]
public class PoolManager : MonoBehaviour
{
    /****** Public Members ******/

    public static PoolManager Instance { get; private set; }

    public void RegisterPool<TEnum, TPoolObject>(Dictionary<TEnum, GameObject> prefabMap, int preloadCount)
        where TEnum : struct, Enum
        where TPoolObject : IPoolable
    {
        var pools = PoolHolder<TEnum, TPoolObject>.Pools;
        Assert.IsTrue(pools.Count == 0, $"Pool for enum '{typeof(TEnum)}' is already registered.");

        foreach (var kv in prefabMap)
        {
            pools.Add(kv.Key, new ObjectPool<TPoolObject>(kv.Value, preloadCount, transform));
        }
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

    private static class PoolHolder<TEnum, TPoolObject>
        where TEnum : struct, Enum
        where TPoolObject : IPoolable
    {
        internal static readonly Dictionary<TEnum, ObjectPool<TPoolObject>> Pools = new();

        internal static TPoolObject Get(TEnum id)
        {
            Assert.IsTrue(Pools.ContainsKey(id), $"No pool registered for ID '{id}'. Did you call RegisterPool?");

            return Pools[id].Get();
        }

        internal static void Return(TEnum id, TPoolObject obj)
        {
            Assert.IsTrue(Pools.ContainsKey(id), $"No pool registered for ID '{id}'. Did you call RegisterPool?");

            Pools[id].Return(obj);
        }
    }
}