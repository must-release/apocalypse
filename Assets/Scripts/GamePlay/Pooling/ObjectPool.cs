using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectPool<T> where T : IPoolable
{
    /****** Public Members ******/

    public ObjectPool(AssetReferenceGameObject objectReference, Transform parent = null)
    {
        _objectReference = objectReference;
        _parent = parent;
    }

    public IEnumerator Preload(int count)
    {
        var loadHandle = _objectReference.LoadAssetAsync<GameObject>();
        yield return loadHandle;

        if (loadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Logger.Write(LogCategory.AssetLoad, $"Failed to load prefab: {_objectReference.RuntimeKey}");
            yield break;
        }

        _loadedPrefab = loadHandle.Result;

        for (int i = 0; i < count; i++)
        {
            var handle = Addressables.InstantiateAsync(_objectReference, _parent);
            yield return handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Logger.Write(LogCategory.AssetLoad, $"Failed to instantiate prefab: {_objectReference.RuntimeKey}");
                yield break;
            }

            handle.Result.SetActive(false);
            Return(handle.Result.GetComponent<T>());
        }
    }

    public T Get()
    {
        T instance = (_pool.Count > 0) ? _pool.Dequeue() : CreateAdditionalInstance();
        instance.OnGetFromPool();

        return instance;
    }

    public void Return(T instance)
    {
        instance.OnReturnToPool();
        _pool.Enqueue(instance);
    }

    /****** Private Members ******/

    private readonly Queue<T>   _pool = new Queue<T>();
    private readonly Transform  _parent;
    private readonly AssetReferenceGameObject _objectReference;

    private GameObject _loadedPrefab;

    private T CreateAdditionalInstance()
    {
        if (null == _loadedPrefab)
        {
            Logger.Write(LogCategory.AssetLoad, $"Tried to instantiate {_objectReference.RuntimeKey} before loading it!");
            return default;
        }

        var go = Object.Instantiate(_loadedPrefab, _parent);
        go.SetActive(false);
        return go.GetComponent<T>();
    }
}
