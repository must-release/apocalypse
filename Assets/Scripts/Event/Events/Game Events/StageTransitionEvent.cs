using System.Collections.Generic;
using UnityEngine.Assertions;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

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

        if (activeEventTypeCounts.ContainsKey(GameEventType.StageTransition))
        {
            return false;
        }

        return true;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue(null != Info, "Event info is not initialized");

        base.PlayEvent();
        AsyncPlayEvent().Forget();
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != Info, "Event info is not set before termination");

        Info.DestroyInfo();
        Info = null;

        GameEventPool<StageTransitionEvent, StageTransitionEventInfo>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private async UniTask AsyncPlayEvent()
    {
        Assert.IsTrue(null != Info, "Event info is not initialized");

        PlayerManager.Instance.GetPlayerData(out ChapterType _, out int _, out PlayerAvatarType playerType);
        PlayerManager.Instance.SetPlayerData(Info.TargetChapter, Info.TargetStage, playerType);

        await SceneController.Instance.AsyncExecuteStageTransition();

        TerminateEvent();
    }
}