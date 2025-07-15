using System.Collections.Generic;
using UnityEngine.Assertions;

/*
 * Stage Transition Event
 */

public class StageTransitionEvent : GameEventBase<StageTransitionEventInfo>
{
    /****** Public Members ******/

    public override bool ShouldBeSaved => false;
    public override bool IsExclusiveEvent => true;
    public override GameEventType EventType => GameEventType.StageTransition;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Assert.IsTrue(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        return activeEventTypeCounts.Count == 0;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue(null != Info, "Event info is not initialized");

        base.PlayEvent();

        var dataLoadEvent = GameEventFactory.CreateDataLoadEvent(Info.TargetChapter, Info.TargetStage);
        GameEventManager.Instance.Submit(dataLoadEvent);

        TerminateEvent();
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != Info, "Event info is not set before termination");

        Info.DestroyInfo();
        Info = null;

        GameEventPool<StageTransitionEvent, StageTransitionEventInfo>.Release(this);

        base.TerminateEvent();
    }
}