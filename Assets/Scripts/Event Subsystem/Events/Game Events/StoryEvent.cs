using UnityEngine;
using StageEnums;
using UIEnums;
using EventEnums;
using System.Collections;
using UnityEngine.Assertions;

[System.Serializable]
[CreateAssetMenu(fileName = "NewStory", menuName = "Event/StoryEvent", order = 0)]
public class StoryEvent : GameEvent
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
        Assert.IsTrue( GameEventType.Story == eventInfo.EventType,      "Wrong event type[" + eventInfo.EventType.ToString() + "]. Should be StoryEventInfo." );
        Assert.IsTrue( eventInfo.IsInitialized,                         "Event info is not initialized" );

        _storyEventInfo = eventInfo as StoryEventInfo;
        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    // TODO : Find solution for UpdateStoryProgress
    public void UpdateStoryProgress((int, int) storyInfo)
    {
        Assert.IsTrue( null != _storyEventInfo, "Story event is not progressing" );

        // readBlockCount = storyInfo.Item1;
        // readEntryCount = storyInfo.Item2;
    }

    // Terminate story event
    public override void TerminateEvent()
    {
        // Tell StoryController to finish story
        StoryController.Instance.FinishStory();

        _storyEventInfo = null;
        StopCoroutine(_eventCoroutine);
    }

    public override GameEventInfo GetEventInfo()
    {
        return _storyEventInfo;
    }


    /****** Private Members ******/

    private Coroutine       _eventCoroutine;
    private StoryEventInfo  _storyEventInfo;

    private IEnumerator PlayEventCoroutine()
    {
        Assert.IsTrue( null != _storyEventInfo, "Event info should be set" );

        // Change to story UI
        UIController.Instance.ChangeBaseUI(BaseUI.Story);

        // Start Story
        InputEventProducer.Instance.LockInput(true);
        string story = "STORY_" + _storyEventInfo.StoryStage.ToString() + '_' + _storyEventInfo.StoryNumber;
        yield return StoryController.Instance.StartStory(story, _storyEventInfo.ReadBlockCount, _storyEventInfo.ReadEntryCount);
        InputEventProducer.Instance.LockInput(false);

        // Wait for story to end
        yield return new WaitWhile( () => StoryController.Instance.IsStoryPlaying );

        // Terminate story event and play next event
        GameEventManager.Instance.TerminateGameEvent(this);
    }
}


[CreateAssetMenu(fileName = "NewStoryEvent", menuName = "EventInfo/StoryEvent", order = 0)]
public class StoryEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public Stage    StoryStage { get { return _storyStage; } private set { _storyStage = value; }}
    public int      StoryNumber { get { return _storyNumber; } private set { _storyNumber = value; }}
    public int      ReadBlockCount { get { return _readBlockCount; } private set { _readBlockCount = value; }}
    public int      ReadEntryCount { get { return _readEntryCount; } private set { _readEntryCount = value; }}
    public bool     IsOnMap { get { return _isOnMap; } private set { _isOnMap = value; }}

    public void Initialize(Stage storyStage, int storyNumber, int readBlockCount, int readEntryCount, bool isOnMap)
    {
        Assert.IsTrue( false == IsInitialized,          "Duplicate initialization of GameEventInfo is not allowed." );
        Assert.IsTrue( Stage.StageCount != storyStage,  "Story stage should be set." );
        Assert.IsTrue( 0 < storyNumber,                 "Stage Number must be positive number." );
        Assert.IsTrue( 0 <= readBlockCount,             "Read block count can not be negative." );
        Assert.IsTrue( 0 <= readEntryCount,             "Read entry count can not be negative." );
        

        StoryStage      = storyStage;
        StoryNumber     = storyNumber;
        ReadBlockCount  = readBlockCount;
        ReadEntryCount  = readEntryCount;
        IsOnMap         = isOnMap;
        IsInitialized   = true;
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.Story;
    }

    protected override void OnValidate()
    {
        if ( Stage.StageCount != StoryStage && 0 < StoryNumber )
            IsInitialized = true;
    }


    /****** Private Members ******/
    [SerializeField] private Stage  _storyStage         = Stage.StageCount;
    [SerializeField] private int    _storyNumber        = 0;
    [SerializeField] private int    _readBlockCount     = 0;
    [SerializeField] private int    _readEntryCount     = 0;
    [SerializeField] private bool   _isOnMap            = false; // If story is played on the map
}