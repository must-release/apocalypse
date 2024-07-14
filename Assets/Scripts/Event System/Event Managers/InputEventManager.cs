using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputEventManager : MonoBehaviour
{
    public static InputEventManager Instance { get; private set; }

    public InputEvent EventPointer { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlayEvent(InputEvent playingEvent)
    {
        playingEvent.ParentEvent = EventPointer;
        EventPointer = playingEvent;

        playingEvent.PlayEvent();
    }
    
}