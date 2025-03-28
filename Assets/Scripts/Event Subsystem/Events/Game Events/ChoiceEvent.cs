using UnityEngine;
using System.Collections.Generic;
using UIEnums;
using EventEnums;
using System.Collections;
using UnityEngine.Assertions;

/*
 * 스토리 진행 중 화면에 선택지를 표시하고, 플레이어가 고른 선택지를 처리하는 ChoiceEvent입니다.
 */  

public class ChoiceEvent : GameEvent
{
    /****** Public Members ******/

    public void SetEventInfo(ChoiceEventInfo eventInfo)
    {
        Assert.IsTrue(eventInfo.IsInitialized, "Event info is not initialized");

        _choiceEventInfo = eventInfo;
    }

    public override bool CheckCompatibility(GameEvent parentEvent, BaseUI baseUI, SubUI subUI)
    {
        if (parentEvent.EventType == GameEventType.Story) // Can be played when story event is playing
            return true;
        
        return false;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue( null != _choiceEventInfo, "Event info is not set");

        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != _choiceEventInfo, "Event info is not set before termination");

        if ( _eventCoroutine != null )
        {
            StopCoroutine( _eventCoroutine );
            _eventCoroutine = null;
        }

        ScriptableObject.Destroy( _choiceEventInfo );
        _choiceEventInfo = null;

        GameEventPool<ChoiceEvent>.Release( this );
    }

    public override GameEventInfo GetEventInfo()
    {
        return _choiceEventInfo;
    }


    /****** Private Members ******/

    private Coroutine       _eventCoroutine     = null;
    private ChoiceEventInfo _choiceEventInfo    = null;

    private IEnumerator PlayEventCoroutine()
    {
        Assert.IsTrue(null != _choiceEventInfo, "Event info is not set");

        // Set choice info and switch to choice UI
        UIController.Instance.SetChoiceInfo(_choiceEventInfo.ChoiceList);
        UIController.Instance.TurnSubUIOn(SubUI.Choice);

        // Wait for player to select a choice
        yield return new WaitUntil( () =>  null != UIController.Instance.GetSelectedChoice() );

        // Get the selected choice and process it
        string selectedChoice       = UIController.Instance.GetSelectedChoice();
        bool shouldGenerateResponse = _choiceEventInfo.ChoiceList == null;

        StoryController.Instance.ProcessSelectedChoice(selectedChoice, shouldGenerateResponse);

        GameEventManager.Instance.TerminateGameEvent(this);
    }
}

[CreateAssetMenu(fileName = "NewChoiceEvent", menuName = "EventInfo/ChoiceEvent", order = 0)]
public class ChoiceEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public List<string> ChoiceList { get { return _choiceList; } private set { _choiceList = value; }}
    
    public void Initialize(List<string> choices)
    {
        Assert.IsTrue( false == IsInitialized,  "Duplicate initialization of GameEventInfo is not allowed." );
        Assert.IsTrue(null != choices,          "Choices cannot be null.");
        Assert.IsTrue(choices.Count > 0,        "Choice list must have at least one item.");

        ChoiceList      = choices;
        IsInitialized   = true;
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.Choice;
    }

    protected override void OnValidate()
    {
        if ( null != ChoiceList && 0 < ChoiceList.Count )
            IsInitialized = true;
    }


    /****** Private Members ******/

    [SerializeField] private List<string> _choiceList;
}