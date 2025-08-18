using System.Collections.Generic;
using UnityEngine;

public class UIChangeEvent : GameEventBase<UIChangeEventInfo>
{
    /****** Public Members ******/

    public override bool            ShouldBeSaved   => false;
    public override GameEventType   EventType       => GameEventType.UIChange;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Debug.Assert(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        return true;
    }

    public override void PlayEvent()
    {
        Debug.Assert(null != Info, "Event info is not initialized");

        base.PlayEvent();

        UIController.Instance.GetCurrentUI(out BaseUI baseUI, out SubUI _);
        if (Info.TargetUI != baseUI)
        {
            UIController.Instance.ChangeBaseUI(Info.TargetUI); 
        }


        TerminateEvent();
    }

    public override void TerminateEvent()
    {
        Debug.Assert(null != Info, "Event info is not set before termination");

        Info.DestroyInfo();
        Info = null;

        GameEventPool<UIChangeEvent, UIChangeEventInfo>.Release(this);

        base.TerminateEvent();
    }
}