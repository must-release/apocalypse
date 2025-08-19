using System.Collections.Generic;
using UnityEngine;
using AD.Camera;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using AD.GamePlay;

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

        _cancellationTokenSource = new CancellationTokenSource();
        PlayEventAsync(_cancellationTokenSource.Token).Forget();
    }

    public override void TerminateEvent()
    {
        Debug.Assert(null != Info, "Event info is not set before termination");

        _cancellationTokenSource.Cancel();

        Info.DestroyInfo();
        Info = null;

        GameEventPool<CameraEvent, CameraEventInfo>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private CancellationTokenSource _cancellationTokenSource;

    private async UniTask PlayEventAsync(CancellationToken cancellationToken)
    {
        Debug.Assert(null != Info, "Event info is not initialized");

        try
        {
            switch (Info.ActionType)
            {
                case CameraActionType.SwitchToCamera:
                    await SwitchToCameraAsync(cancellationToken);
                    break;
                default:
                    Logger.Write(LogCategory.Event, $"Unsupported camera action type: {Info.ActionType}", LogLevel.Error, true);
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            Logger.Write(LogCategory.Event, "Camera event was cancelled", LogLevel.Error, true);
        }
        
        TerminateEvent();
    }

    private async UniTask SwitchToCameraAsync(CancellationToken cancellationToken)
    {
        Debug.Assert(null != Info, "Event info is not initialized");
        Debug.Assert(null != CameraManager.Instance, "CameraManager is not initialized");

        CameraManager.Instance.DeactivateCamera();
        CameraManager.Instance.SetCurrentCameraByName(Info.CameraName);

        Transform target = GetCameraTarget();
        await CameraManager.Instance.ActivateCameraAsync(target, cancellationToken);
    }

    private Transform GetCameraTarget()
    {
        if (Info.IsTargetPlayer)
        {
            var player = SceneController.Instance.PlayerTransform;
            Debug.Assert(null != player, "Player not found in the current stage.");
            return player;
        }
        
        if (false == string.IsNullOrEmpty(Info.TargetName))
        {
            var target = GamePlayManager.Instance.GetStageActorByName(Info.TargetName);
            Debug.Assert(null != target, $"Target actor with name {Info.TargetName} not found.");
            return target.ActorTransform;
        }

        return null;
    }
}