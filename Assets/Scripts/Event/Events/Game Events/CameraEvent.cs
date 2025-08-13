using System.Collections.Generic;
using UnityEngine;
using AD.Camera;
using Cysharp.Threading.Tasks;

/*
 * Camera Control Event
 */

public class CameraEvent : GameEventBase<CameraEventInfo>
{
    /****** Public Members ******/

    public override bool ShouldBeSaved => false;
    public override bool IsExclusiveEvent => false;
    public override GameEventType EventType => GameEventType.Camera;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        return true;
    }

    public override void PlayEvent()
    {
        Debug.Assert(null != Info, "Event info is not initialized");

        base.PlayEvent();
        PlayEventAsync().Forget();
    }

    public override void TerminateEvent()
    {
        Debug.Assert(null != Info, "Event info is not set before termination");

        Info.DestroyInfo();
        Info = null;

        GameEventPool<CameraEvent, CameraEventInfo>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private async UniTask PlayEventAsync()
    {
        Debug.Assert(null != Info, "Event info is not initialized");

        switch (Info.ActionType)
        {
            case CameraActionType.SwitchToCamera:
                await SwitchToCameraAsync();
                break;
            default:
                Logger.Write(LogCategory.Event, $"Unsupported camera action type: {Info.ActionType}", LogLevel.Error, true);
                break;
        }

        TerminateEvent();
    }

    private async UniTask SwitchToCameraAsync()
    {
        Debug.Assert(null != Info, "Event info is not initialized");
        Debug.Assert(null != CameraManager.Instance, "CameraManager is not initialized");

        CameraManager.Instance.DeactivateCamera();
        CameraManager.Instance.SetCurrentCameraByName(Info.TargetName);

        await CameraManager.Instance.ActivateCameraAsync();
    }
}