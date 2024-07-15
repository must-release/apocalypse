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

    public void PlayEvent(InputEvent playingEvent)
    {
        EventList.Add(playingEvent);

        playingEvent.PlayEvent();
    }

    public void TerminateEvent(InputEvent terminatingEvent)
    {
        EventList.Remove(terminatingEvent);
    }
    
}