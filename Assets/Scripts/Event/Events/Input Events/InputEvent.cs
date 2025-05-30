using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputEvent : MonoBehaviour
{
    public KeyCode eventButton;

    // Detect if event button is pressed
    public virtual bool DetectInput()
    {
        return Input.GetKeyDown(eventButton);
    }

    // Check compatibiliry with parent event and current UI
    public virtual bool CheckCompatibility(List<InputEvent> eventList, BaseUI baseUI, SubUI subUI)
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