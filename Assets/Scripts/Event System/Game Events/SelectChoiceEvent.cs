using UnityEngine;
using System.Collections;

[System.Serializable]
[CreateAssetMenu(fileName = "NewSelectChoice", menuName = "Event/SelectChoiceEvent", order = 0)]
public class SelectChoiceEvent : EventBase
{
    public string optionText;

    // Set event Type on load
    public void OnEnable()
    {
        EventType = TYPE.SELECT_CHOICE;
    }

    // Check compatibility with current event
    public override bool CheckCompatibility(EventBase headEvent)
    {
        //if (headEvent.EventType == TYPE.STORY) // Can be played when story event is playing
        //{
        //    return true;
        //}
        //else
        //    return false;
        return true;
    }
}