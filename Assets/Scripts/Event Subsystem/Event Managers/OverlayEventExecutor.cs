using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayEventExecutor : MonoBehaviour
{
    /****** Public Members ******/

    public void Push(GameEvent gameEvent, GameEvent currentExclusiveEvent)
    {
        _overlayStack.Push(gameEvent);
    }

    private IEnumerator RunOverlayStack()
    {
        while (_overlayStack.Count > 0)
        {
            _current = _overlayStack.Pop();
            _current.PlayEvent();
            yield return new WaitUntil(() => _current.IsFinished);
            _current = null;
        }
    }

    public void Update() { } // not used

    public void Terminate(GameEvent gameEvent)
    {
        gameEvent.TerminateEvent();
    }

    public void Clear()
    {
        
    }

    public bool IsIdle => _current == null;


    private readonly Stack<GameEvent> _overlayStack = new();
    private GameEvent _current;
}