using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * EventProducer which creates the input event stream
 */

public class InputEventProducer : MonoBehaviour
{
    public static InputEventProducer Instance { get; private set; }

    private List<InputEvent> inputEventsPool;
    private Queue<InputEvent> incomingEvents;
    private List<InputEvent> playingEvents;
    private bool inputLock;
    private Transform eventCanvas;
    private Transform inputEvents;
    private GameObject clickPreventPanel;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            inputEventsPool = new List<InputEvent>();
            incomingEvents = new Queue<InputEvent>();
            playingEvents = new List<InputEvent>();
            inputLock = false;
            eventCanvas = transform.Find("Event Canvas");
            inputEvents = transform.Find("Input Events");
            clickPreventPanel = eventCanvas.Find("Click Prevent Panel").gameObject;

            // Pool input Events
            PoolInputEvents();
        }
    }

    private void PoolInputEvents()
    {
        for (int i = 0; i < inputEvents.childCount; i++)
        {
            inputEventsPool.Add(inputEvents.GetChild(i).GetComponent<InputEvent>());
        }
    }
 
    private void Update()
    {
        // Check input lock
        if (inputLock) { return; }

        // Detect incoming inputs
        DetectInputEvents();

        // Check compatibility of detected input events
        CheckInputEvents();

        // Handle playable input events
        HandleInputEvents();
    }

    // Lock or Unlock input
    public void LockInput(bool value)
    {
        inputLock = value;
        clickPreventPanel.SetActive(value);
    }

    // Detect every input event.
    private void DetectInputEvents()
    {
        inputEventsPool.ForEach((inputEvent) => {
            if (inputEvent.DetectInput())
            {
                incomingEvents.Enqueue(inputEvent);
            }
        });
    }

    private void CheckInputEvents()
    {
        // Add playable events in playingEvents list
        while (incomingEvents.Count > 0)
        {
            InputEvent input = incomingEvents.Dequeue();
            // bool result = EventChecker.Instance.CheckEventCompatibility(input);
            // if (result)
            // {
            //     playingEvents.Add(input);
            // }
            playingEvents.Add(input);
        }
    }

    // Handle every playable input event
    private void HandleInputEvents()
    {
        playingEvents.ForEach((input) => InputEventManager.Instance.PlayInputEvent(input));
        playingEvents.Clear();
    }
}

