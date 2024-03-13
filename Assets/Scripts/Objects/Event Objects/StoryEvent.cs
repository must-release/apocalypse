using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewStory", menuName = "Event/StoryEvent", order = 0)]
public class StoryEvent : EventBase
{
    public UserData.STAGE stage;
    public int storyNum;
    public bool onMap; // If story is played on the map

    // Set event Type on load
    public void OnEnable() { EventType = TYPE.STORY; }

    public void Initialize(UserData.STAGE stage, int storyNum, EventBase nextEvent)
    {
        this.stage = stage;
        this.storyNum = storyNum;
        NextEvent = nextEvent;
    }
}
