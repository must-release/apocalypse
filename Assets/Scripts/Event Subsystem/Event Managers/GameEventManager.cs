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

    public void Submit(GameEvent gameEvent)
    {
        if (gameEvent.CheckCompatibility(_activeEventTypeCounts))
        {
            Debug.Log($"Activating Event : {gameEvent.GetEventType()}");
            Activate(gameEvent);
        }
        else
        {
            Debug.Log($"Enque Event to Waiting Queue : {gameEvent.GetEventType()}");
            _waitingQueue.Enqueue(gameEvent);
        }
    }

    public List<GameEventInfo> GetSavableEventInfoList()
    {
        List<GameEventInfo> infoList = new();

        foreach (var gameEvent in _activeEvents)
        {
            if (gameEvent.ShouldBeSaved())
                infoList.Add(gameEvent.GetEventInfo());
        }

        return infoList;
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

        if (_activeEventTypeCounts.ContainsKey(gameEvent.GetEventType()))
            _activeEventTypeCounts[gameEvent.GetEventType()]++;
        else
            _activeEventTypeCounts[gameEvent.GetEventType()] = 1;

        gameEvent.OnTerminate += () => HandleEventTermination(gameEvent);
        gameEvent.PlayEvent();
    }

    private void HandleEventTermination(GameEvent gameEvent)
    {
        _activeEvents.Remove(gameEvent);

        if (_activeEventTypeCounts.ContainsKey(gameEvent.GetEventType()))
        {
            _activeEventTypeCounts[gameEvent.GetEventType()]--;
            if (_activeEventTypeCounts[gameEvent.GetEventType()] <= 0)
                _activeEventTypeCounts.Remove(gameEvent.GetEventType());
        }

        TryProcessWaitingQueue();
    }

    private void TryProcessWaitingQueue()
    {
        Queue<GameEvent> tempQueue = new();

        while (_waitingQueue.Count > 0)
        {
            var waiting = _waitingQueue.Dequeue();

            if (waiting.CheckCompatibility(_activeEventTypeCounts))
            {
                Debug.Log($"Process Waiting Event : {waiting.GetEventType()}");
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