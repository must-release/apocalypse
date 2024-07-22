using System.Collections.Generic;
using UnityEngine;
using EventEnums;
using UIEnums;

[System.Serializable]
public class InputEvent
{
    public KeyCode eventButton;

    // Check compatibiliry with parent event and current UI
    public virtual bool CheckCompatibility(List<InputEvent> eventList, (BASEUI, SUBUI) currentUI)
    {
        return default;
    }


    // Play this input event
    public virtual void PlayEvent()
    {
        return;
    }

    // Terminate this input event
    public virtual void TerminateEvent() 
    {
        return;
    }
}