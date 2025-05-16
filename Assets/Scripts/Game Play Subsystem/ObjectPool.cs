using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : IPoolable
{
    /****** Public Members ******/

    public ObjectPool(GameObject prefab, int preloadCount, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
        Preload(preloadCount);
    }

    public T Get()
    {
        T instance = (_pool.Count > 0) ? _pool.Dequeue() : CreateInstance();
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

    private void Preload(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var instance = CreateInstance();
            Return(instance);
        }
    }

    private T CreateInstance()
    {
        var instance = Object.Instantiate(_prefab, _parent);
        instance.SetActive(false);
        return instance.GetComponent<T>();
    }
}
