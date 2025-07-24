using System.Collections.Generic;
using UnityEngine.Assertions;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

/*
 * Fall Death Event
 */

public class FallDeathEvent : GameEventBase<FallDeathEventInfo>
{
    /****** Public Members ******/

    public override bool ShouldBeSaved => false;
    public override bool IsExclusiveEvent => true;
    public override GameEventType EventType => GameEventType.FallDeath;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Assert.IsTrue(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        if (activeEventTypeCounts.ContainsKey(GameEventType.FallDeath))
        {
            return false;
        }

        return true;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue(null != Info, "Event info is not initialized");

        base.PlayEvent();

        SceneController.Instance.ExecutePlayerRespawn();
        TerminateEvent();
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != Info, "Event info is not set before termination");

        Info.DestroyInfo();
        Info = null;

        GameEventPool<FallDeathEvent, FallDeathEventInfo>.Release(this);

        base.TerminateEvent();
    }
}