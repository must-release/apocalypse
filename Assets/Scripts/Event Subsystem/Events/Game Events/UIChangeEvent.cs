using System.Collections.Generic;
using UnityEngine;
using EventEnums;
using UnityEngine.Assertions;
using UnityEngine.AddressableAssets;

public class UIChangeEvent : GameEvent
{
    /****** Public Members ******/

    public override bool            ShouldBeSaved   => false;
    public override GameEventInfo   EventInfo       => _info;
    public override GameEventType   EventType       => GameEventType.UIChange;

    public void SetEventInfo(UIChangeEventInfo eventInfo)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "Event info is not valid.");

        _info   = eventInfo;
        Status  = EventStatus.Waiting;
    }

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Assert.IsTrue(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        return true;
    }

    // Play change UI event
    public override void PlayEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not initialized");

        base.PlayEvent();
        UIController.Instance.ChangeBaseUI(_info.TargetUI); 

        TerminateEvent();
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not set before termination");

        // Todo: Release the event info
        // if (_info.IsFromAddressables) Addressables.Release(_info);
        // else Destroy(_info);
        _info = null;

        GameEventPool<UIChangeEvent>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private UIChangeEventInfo _info = null;
}