using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using EventEnums;

public class GameEventManager : MonoBehaviour
{
    /****** Public Members ******/

    public static GameEventManager Instance { get; private set; }

    public bool IsEventTypeActive(GameEventType type)
    {
        return _activeEventTypeCounts.ContainsKey(type);
    }

    public void Submit(GameEvent gameEvent)
    {
        if (gameEvent.CheckCompatibility())
        {
            Activate(gameEvent);
        }
        else
        {
            _waitingQueue.Enqueue(gameEvent);
        }
    }


    /****** Private Members ******/

    private readonly List<GameEvent> _activeEvents = new();
    private readonly Queue<GameEvent> _waitingQueue = new();
    private readonly Dictionary<GameEventType, int> _activeEventTypeCounts = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Activate(GameEvent gameEvent)
    {
        _activeEvents.Add(gameEvent);

        if (_activeEventTypeCounts.ContainsKey(gameEvent.EventType))
            _activeEventTypeCounts[gameEvent.EventType]++;
        else
            _activeEventTypeCounts[gameEvent.EventType] = 1;

        gameEvent.OnTerminate += () => HandleEventTermination(gameEvent);
        gameEvent.PlayEvent();
    }

    private void HandleEventTermination(GameEvent gameEvent)
    {
        _activeEvents.Remove(gameEvent);

        if (_activeEventTypeCounts.ContainsKey(gameEvent.EventType))
        {
            _activeEventTypeCounts[gameEvent.EventType]--;
            if (_activeEventTypeCounts[gameEvent.EventType] <= 0)
                _activeEventTypeCounts.Remove(gameEvent.EventType);
        }

        TryProcessWaitingQueue();
    }

    private void TryProcessWaitingQueue()
    {
        Queue<GameEvent> tempQueue = new();

        while (_waitingQueue.Count > 0)
        {
            var waiting = _waitingQueue.Dequeue();

            if (waiting.CheckCompatibility())
            {
                Activate(waiting);
            }
            else
            {
                tempQueue.Enqueue(waiting);
            }
        }

        // Restore unresolved events
        while (0 < tempQueue.Count)
        {
            _waitingQueue.Enqueue(tempQueue.Dequeue());
        }
    }
}