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
    private List<InputEvent> playingEvents;
    private CancelEvent cancelEvent;
    private PauseEvent pauseEvent;
    private ConfirmEvent confirmEvent;

    private KeyCode cancelButton;
    private KeyCode pauseButton;
    private KeyCode confirmButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            inputEvents = new Queue<InputEvent>();
            playingEvents = new List<InputEvent>();
            cancelEvent = new CancelEvent();
            pauseEvent = new PauseEvent();
            confirmEvent = new ConfirmEvent();
        }
    }

    private void Start()
    {
        // Input event producer observes preference manager
        PreferenceManager.Instance.AddObserver(this);
    }

    // Detect every input event. 
    private void Update()
    {
        if (Input.GetKeyDown(cancelButton)) { inputEvents.Enqueue(cancelEvent); }

        if (Input.GetKeyDown(pauseButton)) { inputEvents.Enqueue(pauseEvent); }

        if (Input.GetKeyDown(confirmButton)) { inputEvents.Enqueue(confirmEvent); }

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
                playingEvents.Add(input);
            }
        }

        playingEvents.ForEach((input) => InputEventManager.Instance.PlayEvent(input));
        playingEvents.Clear();
    }

    // Get Updated Preference
    public void PreferenceUpdated()
    {
        PreferenceManager.KeySettings keySettings = PreferenceManager.Instance.KeySettingInfo;


        if (keySettings != null)
        {
            cancelButton = keySettings.cancelButton;
            pauseButton = keySettings.pauseButton;
            confirmButton = keySettings.confirmButton;

            // Debug.Log("Key Settings updated:");
            // Debug.Log("Cancel: " + cancelButton);
            // Debug.Log("Pause: " + pauseButton);
            // Debug.Log("Confirm: " + confirmButton);
        }
        else
        {
            Debug.LogError("Failed to load key settings.");
        }
    }
}

