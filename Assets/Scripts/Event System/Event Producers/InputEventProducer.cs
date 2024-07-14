using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * EventProducer which creates the input event stream
 */

public class InputEventProducer : MonoBehaviour, PreferenceObserver
{
    public static InputEventProducer Instance { get; private set; }

    private Queue<InputEvent> inputEvents;

    private KeyCode cancelButton;
    private KeyCode pauseButton;
    private KeyCode confirmButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            inputEvents = new Queue<InputEvent>();
        }
    }

    private void Start()
    {
        // Input event producer observes preference manager
        PreferenceManager.Instance.AddObserver(this);
    }

    // Detect every input event. The event higher up has a higher priority.
    private void Update()
    {
        if (Input.GetKeyDown(cancelButton))
        {
            inputEvents.Enqueue(new CancelEvent());
        }

        if (Input.GetKeyDown(pauseButton))
        {
            inputEvents.Enqueue(new PauseEvent());
        }

        if (Input.GetKeyDown(confirmButton))
        {
            inputEvents.Enqueue(new ConfirmEvent());
        }

        // Handle every generated input event
        HandleGeneratedEvents();
    }

    // Handle generated Input events to InputEventManager
    private void HandleGeneratedEvents()
    {
        while (inputEvents.Count > 0)
        {
            InputEvent input = inputEvents.Dequeue();
            bool result = EventChecker.Instance.CheckEventCompatibility(input);
            if (result)
            {
                InputEventManager.Instance.PlayEvent(input);
            }
        }
        InputEventManager.Instance.EventPointer = null;
    }

    // Get Updated Preference
    public void PreferenceUpdated()
    {
        KeySettings keySettings = PreferenceManager.Instance.keySettings;

        cancelButton = keySettings.cancelButton;
        pauseButton = keySettings.pauseButton;
        confirmButton = keySettings.confirmButton;
    }
}

