using System.Collections.Generic;
using UnityEngine;

public class ExclusiveEventExecutor
{
    public GameEvent CurrentEvent => _current;

    private Queue<GameEvent> _queue = new Queue<GameEvent>();
    private GameEvent _current;

    public void Submit(GameEvent gameEvent)
    {
        _queue.Enqueue(gameEvent);
    }

    public void Update()
    {
        if (_current == null && _queue.Count > 0)
        {
            _current = _queue.Dequeue();
            _current.PlayEvent();
        }
    }

    public void Terminate(GameEvent gameEvent)
    {
        if (_current == gameEvent)
        {
            _current.TerminateEvent();
            _current = null;
        }
    }

    public void Clear()
    {
        
    }

    public bool IsIdle => _current == null;
}
