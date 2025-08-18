using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AD.Camera;
using AD.GamePlay;
using AD.UI;

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

            var controlUI = UIController.Instance.GetUIView(BaseUI.Control) as ControlUIView;
            Debug.Assert(null != controlUI, "Cannot find control ui in scene activate event.");

            var playerController = player.GetComponent<PlayerController>();
            Debug.Assert(null != playerController, "PlayerController is not find in player transform in scene activate event.");

            playerController.OnHPChanged += controlUI.UpdateHPBar;
            controlUI.UpdateHPBar(playerController.CurrentHitPoint, playerController.MaxHitPoint);
        }

        var sceneCameras = SceneController.Instance.GetCurrentSceneCameras();
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

        var actors = SceneController.Instance.GetCurrentStageActors();
        if (0 < actors.Length)
        {
            GamePlayManager.Instance.RegisterStageActors(actors);
        }

        SceneController.Instance.ActivateGameScene();

        TerminateEvent();
    }
}