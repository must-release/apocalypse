using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputEventManager : MonoBehaviour
{
    public static InputEventManager Instance { get; private set; }

    public List<InputEvent> EventList { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            EventList = new List<InputEvent>();
        }
    }

    // Play input event
    public void PlayInputEvent(InputEvent playingEvent)
    {
        EventList.Add(playingEvent);

        playingEvent.PlayEvent();
    }

    // Terminate target event
    public void TerminateInputEvent(InputEvent terminatingEvent)
    {
        EventList.Remove(terminatingEvent);

        terminatingEvent.TerminateEvent();
    }
    
}