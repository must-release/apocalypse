using UnityEngine;
using System.Collections.Generic;
using AD.Audio;

public class BGMEvent : GameEventBase<BGMEventInfo>
{
    public override bool ShouldBeSaved      => false;
    public override GameEventType EventType => GameEventType.BGM;
    public override bool IsExclusiveEvent   => false;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        return true;
    }

    public override void PlayEvent()
    {
        Debug.Assert(null != Info, "Event info is not initialized");

        base.PlayEvent();

        if (Info.ShouldStop)
        {
            AudioManager.Instance.StopBGM();
        }
        else
        {
            AudioManager.Instance.PlayBGM(Info.ClipName);
        }

        TerminateEvent();
    }

    public override void TerminateEvent()
    {
        Debug.Assert(null != Info, "Event info is not set before termination");

        Info.DestroyInfo();
        Info = null;

        GameEventPool<BGMEvent, BGMEventInfo>.Release(this);

        base.TerminateEvent();
    }
}
