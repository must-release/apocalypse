using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObject : MonoBehaviour
{
    public ScriptableObject playingEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EventManager.Instance.PlayEvent((IEvent)playingEvent);
    }
}
