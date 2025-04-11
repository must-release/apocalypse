using UnityEngine;
using EventEnums;
using UnityEngine.Assertions;
using System;

[Serializable]
[CreateAssetMenu(fileName = "NewCutsceneEvent", menuName = "EventInfo/CutsceneEvent", order = 0)]
public class CutsceneEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public void Initialize()
    {
        Assert.IsTrue(false == IsInitialized, "Duplicate initialization of GameEventInfo is not allowed.");

        IsInitialized = true;
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.Cutscene;
    }

    protected override void OnValidate()
    {
        IsInitialized = true;
    }


    /****** Private Members ******/
}