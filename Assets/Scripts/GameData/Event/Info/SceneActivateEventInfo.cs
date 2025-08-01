﻿using UnityEngine;
using UnityEngine.Assertions;
using System;


[Serializable]
[CreateAssetMenu(fileName = "NewSceneActivateEventInfo", menuName = "EventInfo/SceneActivateEvent", order = 0)]
public class SceneActivateEventInfo : GameEventInfo, ISerializableEventInfo
{
    /****** Public Members ******/

    public bool ShouldTurnOnLoadingUI { get => _shouldTurnOnLoadingUI; private set => _shouldTurnOnLoadingUI = value; }

    public void Initialize(bool shouldTurnOnLoadingUI, bool isRuntimeInstance = false)
    {
        Debug.Assert(false == IsInitialized, "Duplicate initialization of GameEventInfo is not allowed.");

        ShouldTurnOnLoadingUI   = shouldTurnOnLoadingUI;
        IsInitialized           = true;
        IsRuntimeInstance       = true;
    }

    public override GameEventInfo Clone()
    {
        var clone = Instantiate(this);
        clone.IsRuntimeInstance = true;

        return clone;
    }

    public GameEventDTO ToDTO()
    {
        return new SceneActivateEventDTO
        {
            EventType = EventType,
            ShouldTurnOnLoadingUI = _shouldTurnOnLoadingUI
        };
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.SceneActivate;
    }

    protected override void OnValidate()
    {
        IsInitialized = true;
    }


    /****** Private Members ******/

    [SerializeField] private bool _shouldTurnOnLoadingUI = true;
}