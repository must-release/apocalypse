using UnityEngine;
using UnityEngine.Assertions;
using System;

[Serializable]
[CreateAssetMenu(fileName = "NewCutsceneEventInfo", menuName = "EventInfo/CutsceneEvent", order = 0)]
public class CutsceneEventInfo : GameEventInfo, ISerializableEventInfo
{
    /****** Public Members ******/

    public void Initialize(bool isRuntimeInstance = false)
    {
        Assert.IsTrue(false == IsInitialized, "Duplicate initialization of GameEventInfo is not allowed.");

        IsInitialized       = true;
        IsRuntimeInstance   = true;
    }

    public override GameEventInfo Clone()
    {
        var clone = Instantiate(this);
        clone.IsRuntimeInstance = true;

        return clone;
    }

    public GameEventDTO ToDTO()
    {
        return new CutsceneEventDTO
        {
            EventType = EventType
        };
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