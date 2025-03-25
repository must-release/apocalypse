using UnityEngine;
using System.Collections;
using UIEnums;
using EventEnums;
using UnityEngine.Assertions;

/*
 * 게임 플레이 도중 컷씬을 재생하는 CutsceneEvent를 관리하는 파일입니다.
 */

public class CutsceneEvent : GameEvent
{
    /****** Public Members ******/

    public override bool CheckCompatibility(GameEvent parentEvent, BaseUI baseUI, SubUI subUI)
    {
        if ( null == parentEvent )
            return true;
        
        return false;
    }

    public override void PlayEvent(GameEventInfo eventInfo)
    {
        Assert.IsTrue( GameEventType.Cutscene == eventInfo.EventType,   "Wrong event type[" + eventInfo.EventType.ToString() + "]. Should be CutsceneEventInfo.");
        Assert.IsTrue( eventInfo.IsInitialized,                         "Event info is not initialized");

        _choiceEventInfo    = eventInfo as ChoiceEventInfo;
        _eventCoroutine     = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        _choiceEventInfo = null;
        StopCoroutine(_eventCoroutine);
    }

    public override GameEventInfo GetEventInfo()
    {
        return _choiceEventInfo;
    }


    /****** Private Members ******/

    private Coroutine       _eventCoroutine;
    private GameEventInfo   _choiceEventInfo;

    private IEnumerator PlayEventCoroutine()
    {
        // Change to cutscene UI
        UIController.Instance.ChangeBaseUI(BaseUI.Cutscene);

        // Play cutscene
        GamePlayManager.Instance.PlayCutscene();

        // Wait for cutscene to end
        yield return new WaitUntil( () => GamePlayManager.Instance.IsCutscenePlaying );

        // Terminate cutscene event
        GameEventManager.Instance.TerminateGameEvent(this);
    }
}


[CreateAssetMenu(fileName = "NewCutsceneEvent", menuName = "EventInfo/CutsceneEvent", order = 0)]
public class CutsceneEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public void Initialize()
    {
        Assert.IsTrue(false == IsInitialized, "Duplicate initialization of GameEventInfo is not allowed.");

        IsInitialized = true;
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.Cutscene;
    }

    protected override void OnValidate()
    {
        IsInitialized = true;
    }


    /****** Private Members ******/
}