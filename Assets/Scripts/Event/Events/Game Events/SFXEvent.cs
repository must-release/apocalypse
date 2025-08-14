using UnityEngine;
using System.Collections.Generic;
using AD.Audio;

public class SFXEvent : GameEventBase<SFXEventInfo>
{
    public override bool ShouldBeSaved      => false;
    public override GameEventType EventType => GameEventType.SFX;
    public override bool IsExclusiveEvent   => false;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        return true;
    }

    public override void PlayEvent()
    {
        Debug.Assert(null != Info, "Event info is not initialized");

        base.PlayEvent();

        AudioManager.Instance.PlaySFX(Info.ClipName);

        TerminateEvent();
    }

    public override void TerminateEvent()
    {
        Debug.Assert(null != Info, "Event info is not set before termination");

        Info.DestroyInfo();
        Info = null;

        GameEventPool<SFXEvent, SFXEventInfo>.Release(this);

        base.TerminateEvent();
    }
}
