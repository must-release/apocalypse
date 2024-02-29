using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewStory", menuName = "Event/StoryEvent", order = 0)]
public class StoryEvent : ScriptableObject, IEvent
{
    public IEvent.TYPE EventType { get; set; } = IEvent.TYPE.STORY;
    public IEvent NextEvent { get => (IEvent)nextEvent; set => nextEvent = (Object)value; }
    public Object nextEvent;
    public UserData.STAGE stage;
    public int storyNum;
}
