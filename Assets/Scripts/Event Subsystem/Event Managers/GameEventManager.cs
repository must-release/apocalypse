using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using EventEnums;

public class GameEventManager : MonoBehaviour
{
    /****** Public Members ******/

    public static GameEventManager Instance { get; private set; }

    public bool IsEventTypeActive(GameEventType eventType)
    {
        return _activeEventTypeCounts.ContainsKey(eventType);
    }

    public IGameEvent GetActiveEvent(GameEventType eventType)
    {
        foreach (var gameEvent in _activeEvents)
        {
            if (gameEvent.EventType == eventType)
                return gameEvent;
        }

        return null;
    }

    public void Submit(IGameEvent gameEvent)
    {
        if (gameEvent.CheckCompatibility(_activeEventTypeCounts))
        {
            Debug.Log($"Activating Event : {gameEvent.EventType}");
            Activate(gameEvent);
        }
        else
        {
            Debug.Log($"Enque Event to Waiting Queue : {gameEvent.EventType}");
            _waitingQueue.Enqueue(gameEvent);
        }
    }

    public List<GameEventInfo> GetSavableEventInfoList()
    {
        List<GameEventInfo> infoList = new();

        foreach (var gameEvent in _activeEvents)
        {
            if (gameEvent.ShouldBeSaved)
                infoList.Add(gameEvent.EventInfo);
        }

        return infoList;
    }


    /****** Private Members ******/

    private readonly List<IGameEvent> _activeEvents = new();
    private readonly Queue<IGameEvent> _waitingQueue = new();
    private readonly Dictionary<GameEventType, int> _activeEventTypeCounts = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Activate(IGameEvent gameEvent)
    {
        _activeEvents.Add(gameEvent);

        if (_activeEventTypeCounts.ContainsKey(gameEvent.EventType))
            _activeEventTypeCounts[gameEvent.EventType]++;
        else
            _activeEventTypeCounts[gameEvent.EventType] = 1;

        gameEvent.OnTerminate += () => HandleEventTermination(gameEvent);
        gameEvent.PlayEvent();
    }

    private void HandleEventTermination(IGameEvent gameEvent)
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
        Queue<IGameEvent> tempQueue = new();

        while (_waitingQueue.Count > 0)
        {
            var waiting = _waitingQueue.Dequeue();

            if (waiting.CheckCompatibility(_activeEventTypeCounts))
            {
                Debug.Log($"Process Waiting Event : {waiting.EventType}");
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