﻿using UnityEngine;
using System.Collections.Generic;

public static class GameEventPool<TEvent, TInfo> 
    where TEvent : GameEventBase<TInfo>
    where TInfo : GameEventInfo
{
    private static readonly Stack<TEvent> _pool = new Stack<TEvent>();
    private static Transform _poolParent;

    static GameEventPool()
    {
        var host = GameObject.Find("Event System");
        if (host == null)
        {
            Debug.LogError("Event System not found in scene.");
            return;
        }

        var pooledRoot = GameObject.Find("Pooled Events");
        if (pooledRoot == null)
        {
            pooledRoot = new GameObject("Pooled Events");
            pooledRoot.transform.SetParent(host.transform, false);
        }

        _poolParent = pooledRoot.transform;
    }

    public static TEvent Get(Transform activeParent, string name)
    {
        if (_pool.Count > 0)
        {
            var evt = _pool.Pop();
            evt.transform.SetParent(activeParent);
            evt.gameObject.name = name;
            evt.gameObject.SetActive(true);
            return evt;
        }

        var newEvt = new GameObject(name);
        newEvt.transform.SetParent(activeParent);
        return newEvt.AddComponent<TEvent>();
    }

    public static void Release(TEvent evt)
    {
        evt.gameObject.SetActive(false);
        evt.transform.SetParent(_poolParent);
        _pool.Push(evt);
    }
}