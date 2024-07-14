using UnityEngine;
using System.Collections.Generic;
using UIEnums;
using EventEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewChoice", menuName = "Event/ChoiceEvent", order = 0)]
public class ChoiceEvent : GameEvent
{
    public List<string> choiceList;
    public string selectedChoice;


    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.CHOICE;
    }

    // Check compatibility with current event
    public override bool CheckCompatibility(GameEvent parentEvent, (BASEUI, SUBUI) currentUI)
    {
        if (parentEvent.EventType == EVENT_TYPE.STORY) // Can be played when story event is playing
        {
            return true;
        }
        else
            return false;
    }
}