using UnityEngine;
using System.Collections;

[System.Serializable]
[CreateAssetMenu(fileName = "NewShowChoice", menuName = "Event/ShowChoiceEvent", order = 0)]
public class ShowChoiceEvent : EventBase
{

    // Set event Type on load
    public void OnEnable()
    {
        EventType = TYPE.SHOW_CHOICE;
    }

    // Check compatibility with current event
    public override bool CheckCompatibility(EventBase headEvent)
    {
        if (headEvent.EventType == TYPE.STORY) // Can be played when story event is playing
        {
            return true;
        }
        else
            return false;
    }
}