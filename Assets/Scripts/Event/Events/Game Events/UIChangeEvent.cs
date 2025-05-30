using System.Collections.Generic;

using UnityEngine.Assertions;
using UnityEngine.AddressableAssets;

public class UIChangeEvent : GameEventBase<UIChangeEventInfo>
{
    /****** Public Members ******/

    public override bool            ShouldBeSaved   => false;
    public override GameEventType   EventType       => GameEventType.UIChange;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Assert.IsTrue(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        return true;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue(null != Info, "Event info is not initialized");

        base.PlayEvent();
        UIController.Instance.ChangeBaseUI(Info.TargetUI); 

        TerminateEvent();
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != Info, "Event info is not set before termination");

        Info.DestroyInfo();
        Info = null;

        GameEventPool<UIChangeEvent, UIChangeEventInfo>.Release(this);

        base.TerminateEvent();
    }
}