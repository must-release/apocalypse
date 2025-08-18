using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using AD.Camera;

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
        Debug.Assert(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        if (activeEventTypeCounts.ContainsKey(GameEventType.StageTransition))
        {
            return false;
        }

        return true;
    }

    public override void PlayEvent()
    {
        Debug.Assert(null != Info, "Event info is not initialized");

        base.PlayEvent();
        AsyncPlayEvent().Forget();
    }

    public override void TerminateEvent()
    {
        Debug.Assert(null != Info, "Event info is not set before termination");

        Info.DestroyInfo();
        Info = null;

        GameEventPool<StageTransitionEvent, StageTransitionEventInfo>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private async UniTask AsyncPlayEvent()
    {
        Debug.Assert(null != Info, "Event info is not initialized");

        PlayerManager.Instance.GetPlayerData(out ChapterType _, out int _, out PlayerAvatarType playerType);
        PlayerManager.Instance.SetPlayerData(Info.TargetChapter, Info.TargetStage, playerType);

        await SceneController.Instance.ExecuteStageTransitionAsync();

        var newSceneCameras = SceneController.Instance.GetCurrentSceneCameras();
        Transform player = SceneController.Instance.PlayerTransform;

        if (0 < newSceneCameras.Length)
        {
            CameraManager.Instance.ClearCameras();
            CameraManager.Instance.RegisterCameras(newSceneCameras);

            if (null != player)
            {
                CameraManager.Instance.SetCurrentCamera<FollowCamera>();
                CameraManager.Instance.ActivateCamera(player);
            }
            else
            {
                CameraManager.Instance.SetCurrentCamera(newSceneCameras[0]);
            }
        }

        TerminateEvent();
    }
}