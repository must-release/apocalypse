using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectPool<T> where T : IPoolable
{
    /****** Public Members ******/

    public ObjectPool(GameObject prefab, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
    }

    public IEnumerator Preload(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var handle = Addressables.InstantiateAsync(_prefab, _parent);
            yield return handle; 

            if (AsyncOperationStatus.Failed == handle.Status)
            {
                Logger.Write(LogCategory.AssetLoad, $"Failed to instatiate {_prefab.name}.");
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
    private readonly GameObject _prefab;
    private readonly Transform  _parent;

    private T CreateAdditionalInstance()
    {
        Logger.Write(LogCategory.AssetLoad, $"Loading additional {_prefab.name} instance! This may cause frame drop.");

        var instance = Object.Instantiate(_prefab, _parent);
        instance.SetActive(false);
        return instance.GetComponent<T>();
    }
}
