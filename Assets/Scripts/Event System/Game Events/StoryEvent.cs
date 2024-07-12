using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StageEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewStory", menuName = "Event/StoryEvent", order = 0)]
public class StoryEvent : EventBase
{
    public STAGE stage;
    public int storyNum;
    public int readBlockCount;
    public int readEntryCount;

    public bool onMap; // If story is played on the map

    // Set event Type on load
    public void OnEnable()
    {
        EventType = TYPE.STORY;
    }
}
