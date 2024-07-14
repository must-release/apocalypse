using System;
using UnityEngine;
using EventEnums;
using UIEnums;

[System.Serializable]
public class InputEvent
{
    public InputEvent ParentEvent { get; set; }

    // Check compatibiliry with parent event and current UI
    public virtual bool CheckCompatibility(InputEvent parentEvent, (BASEUI, SUBUI) currentUI)
    {
        return default;
    }


    // Play this input event
    public virtual void PlayEvent()
    {
        return;
    }
}