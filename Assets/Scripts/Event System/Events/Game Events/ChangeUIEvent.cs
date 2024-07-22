using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIEnums;
using EventEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewChangeUI", menuName = "Event/ChangeUI", order = 0)]
public class ChangeUIEvent : GameEvent
{
    public BASEUI changingUI;

    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.CHANGE_UI;
    }

    // Check compatibility with current event and UI
    public override bool CheckCompatibility(GameEvent parentEvent, (BASEUI, SUBUI) currentUI)
    {
        // Can be played when there is no event playing
        if (parentEvent == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Play change UI event
    public override void PlayEvent()
    {
        // Change UI
        UIController.Instance.ChangeBaseUI(changingUI); 

        // Terminate change UI event
        GameEventManager.Instance.TerminateGameEvent(this);
    }
}