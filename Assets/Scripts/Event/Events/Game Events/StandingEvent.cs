using System.Collections.Generic;
using UnityEngine;

public class StandingEvent : GameEventBase<StandingEventInfo>
{
    public override bool ShouldBeSaved => false;
    public override GameEventType EventType => GameEventType.Standing;

    public override bool CheckCompatibility(System.Collections.Generic.IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        return true;
    }

    public override void PlayEvent()
    {
        base.PlayEvent();
        
        TerminateEvent();
    }
}