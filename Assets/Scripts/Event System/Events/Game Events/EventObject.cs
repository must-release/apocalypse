using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObject : MonoBehaviour
{
    public GameEvent playingEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameEventManager.Instance.StartEventChain(playingEvent);
    }
}
