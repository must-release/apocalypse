using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObject : MonoBehaviour
{
    public EventBase playingEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameEventRouter.Instance.PlayEvent(playingEvent);
    }
}
