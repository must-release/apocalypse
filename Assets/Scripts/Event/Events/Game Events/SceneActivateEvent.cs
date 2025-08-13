using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

/*
 * Activate loaded scene
 */

public class SceneActivateEvent : GameEventBase<SceneActivateEventInfo>
{
    /****** Public Members *****/

    public override bool            ShouldBeSaved   => false;
    public override GameEventType   EventType       => GameEventType.SceneActivate;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Debug.Assert(null != activeEventTypeCounts, "activeEventTypeCounts is null.");

        foreach (GameEventType eventType in activeEventTypeCounts.Keys)
        {
            if (GameEventType.Story == eventType || GameEventType.Sequential == eventType)
                continue;

            return false;
        }

        return true;
    }

    public override void PlayEvent()
    {
        Debug.Assert(null != Info, "Event info is not set.");

        base.PlayEvent();
        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        Debug.Assert(false == SceneController.Instance.IsSceneLoading, "Should not be terminated when scene is not loaded yet.");
        Debug.Assert(null != Info, "Event info is not set before termination");

        if (_eventCoroutine != null)
        {
            StopCoroutine(_eventCoroutine);
            _eventCoroutine = null;
        }

        Info.DestroyInfo();
        Info = null;

        GameEventPool<SceneActivateEvent, SceneActivateEventInfo>.Release(this);

        base.TerminateEvent();
    }


    /****** Private Members ******/

    private Coroutine _eventCoroutine = null;

    private IEnumerator PlayEventCoroutine()
    {
        Debug.Assert(null != Info, "Event info is not set.");


        if (SceneController.Instance.IsSceneLoading)
        {
            if (Info.ShouldTurnOnLoadingUI)
                UIController.Instance.ChangeBaseUI(BaseUI.Loading);

            yield return new WaitWhile(() => SceneController.Instance.IsSceneLoading);
        }

        Transform player = SceneController.Instance.PlayerTransform;
        if (player)
        {
            PlayerAvatarType character = PlayerManager.Instance.CurrentPlayerType;
            GamePlayManager.Instance.InitializePlayerCharacter(player, character);
        }

        var sceneCameras = SceneController.Instance.GetCurrentStageCameras();
        if (0 < sceneCameras.Length)
        {
            CameraManager.Instance.RegisterCameras(sceneCameras);
            
            if (null != player)
            {
                CameraManager.Instance.SetCurrentCamera<FollowCamera>();
                CameraManager.Instance.ActivateCamera(player);
            }
            else
            {
                CameraManager.Instance.SetCurrentCamera(sceneCameras[0]);
            }
        }

        SceneController.Instance.ActivateGameScene();

        TerminateEvent();
    }
}