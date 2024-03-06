using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewStory", menuName = "Event/StoryEvent", order = 0)]
public class StoryEvent : IEvent
{
    public UserData.STAGE stage;
    public int storyNum;
    public bool onMap; // If story is played on the map

    public void Initialize(UserData.STAGE stage, int storyNum, IEvent nextEvent)
    {
        EventType = TYPE.STORY;
        this.stage = stage;
        this.storyNum = storyNum;
        NextEvent = nextEvent;
    }
}
