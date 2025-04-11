using UnityEngine;
using StageEnums;
using EventEnums;
using UnityEngine.Assertions;
using System;


[Serializable]
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