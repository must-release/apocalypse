using UnityEngine;
using UnityEngine.Assertions;
using System;


[Serializable]
[CreateAssetMenu(fileName = "NewStoryEventInfo", menuName = "EventInfo/StoryEvent", order = 0)]
public class StoryEventInfo : GameEventInfo, ISerializableEventInfo
{
    /****** Public Members ******/

    public ChapterType  StoryStage      => _storyStage;
    public int          StoryNumber     => _storyNumber;
    public int          ReadBlockCount  => _readBlockCount;
    public int          ReadEntryCount  => _readEntryCount;  
    public bool         IsOnMap         => _isOnMap;    

    public void Initialize(ChapterType storyStage, int storyNumber, int readBlockCount, int readEntryCount, bool isOnMap, bool isRuntimeInstance = false)
    {
        Debug.Assert( false == IsInitialized,          "Duplicate initialization of GameEventInfo is not allowed." );
        Debug.Assert( ChapterType.ChapterTypeCount != storyStage,  "Story chapter should be set." );
        Debug.Assert( 0 <= storyNumber,                "Story number can not be negative.");
        Debug.Assert( 0 <= readBlockCount,             "Read block count can not be negative." );
        Debug.Assert( 0 <= readEntryCount,             "Read entry count can not be negative." );
        

        _storyStage         = storyStage;
        _storyNumber        = storyNumber;
        _readBlockCount     = readBlockCount;
        _readEntryCount     = readEntryCount;
        _isOnMap            = isOnMap;
        IsInitialized       = true;
        IsRuntimeInstance   = true;
    }

    public void UpdateStoryProgress(int readBlockCount, int readEntryCount)
    {
        _readBlockCount = readBlockCount;
        _readEntryCount = readEntryCount;
    }

    public override GameEventInfo Clone()
    {
        var clone = Instantiate(this);
        clone.IsRuntimeInstance = true;

        return clone;
    }

    public GameEventDTO ToDTO()
    {
        return new StoryEventDTO
        {
            EventType         = EventType,
            StoryStage        = _storyStage,
            StoryNumber       = _storyNumber,
            ReadBlockCount    = _readBlockCount,
            ReadEntryCount    = _readEntryCount,
            IsOnMap           = _isOnMap
        };
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.Story;
    }

    protected override void OnValidate()
    {
        if ( ChapterType.ChapterTypeCount != StoryStage && 0 < StoryNumber )
            IsInitialized = true;
    }


    /****** Private Members ******/
    [SerializeField] private ChapterType    _storyStage         = ChapterType.ChapterTypeCount;
    [SerializeField] private int            _storyNumber        = 0;
    [SerializeField] private int            _readBlockCount     = 0;
    [SerializeField] private int            _readEntryCount     = 0;
    [SerializeField] private bool           _isOnMap            = false; // If story is played on the map
}