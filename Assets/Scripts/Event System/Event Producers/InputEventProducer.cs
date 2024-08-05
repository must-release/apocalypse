using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * EventProducer which creates the input event stream
 */

public class InputEventProducer : MonoBehaviour, KeySettingsObserver
{
    public static InputEventProducer Instance { get; private set; }

    private List<InputEvent> inputEvents;
    private Queue<InputEvent> incomingEvents;
    private List<InputEvent> playingEvents;
    private bool inputLock;
    private GameObject clickPreventPanel;

    // Input events
    private CancelEvent cancelEvent;
    private PauseEvent pauseEvent;
    private NextScriptEvent nextScriptEvent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            inputEvents = new List<InputEvent>();
            incomingEvents = new Queue<InputEvent>();
            playingEvents = new List<InputEvent>();
            inputLock = false;
            clickPreventPanel = GameObject.Find("Click Prevent Panel");

            // Pool input Events
            inputEvents.Add(cancelEvent = new CancelEvent());
            inputEvents.Add(pauseEvent = new PauseEvent());
            inputEvents.Add(nextScriptEvent = new NextScriptEvent());
        }
    }

    private void Start()
    {
        // Input event producer observes preference manager
        SettingsManager.Instance.AddObserver(this);
    }
 
    private void Update()
    {
        if (inputLock)
        {
            return;
        }

        // Detect every input event.
        inputEvents.ForEach((inputEvent) => {
            if (Input.GetKeyDown(inputEvent.eventButton))
            {
                incomingEvents.Enqueue(inputEvent);
            }
        });

        // Handle every generated input event
        HandleGeneratedEvents();
    }

    // Handle generated Input events to InputEventManager
    private void HandleGeneratedEvents()
    {
        // Add playable events in playingEvents list
        while (incomingEvents.Count > 0)
        {
            InputEvent input = incomingEvents.Dequeue();
            bool result = EventChecker.Instance.CheckEventCompatibility(input);
            if (result)
            {
                playingEvents.Add(input);
            }
        }

        // Play playable events
        playingEvents.ForEach((input) => InputEventManager.Instance.PlayInputEvent(input));
        playingEvents.Clear();
    }

    // Lock or Unlock input
    public void LockInput(bool value)
    {
        inputLock = value;
        clickPreventPanel.SetActive(value);
    }


    // Get Updated Preference
    public void KeySettingsUpdated()
    {
        KeySettings keySettings = SettingsManager.Instance.KeySettingInfo;


        if (keySettings != null)
        {
            cancelEvent.eventButton = keySettings.cancelButton;
            pauseEvent.eventButton = keySettings.pauseButton;
            nextScriptEvent.eventButton = keySettings.confirmButton;

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

