using UnityEngine;
using StageEnums;
using UIEnums;
using EventEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewStory", menuName = "Event/StoryEvent", order = 0)]
public class StoryEvent : GameEvent
{
    public STAGE stage;
    public int storyNum;
    public int readBlockCount;
    public int readEntryCount;

    public bool onMap; // If story is played on the map

    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.STORY;
    }
}
