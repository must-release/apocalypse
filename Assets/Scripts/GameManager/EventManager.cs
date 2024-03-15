using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EventManager : MonoBehaviour
{
	public static EventManager Instance { get; private set; }

	public EventBase CurrentEvent { get; private set; }
	public EventBase NextEvent { get { return CurrentEvent.NextEvent; } }


	void Awake()
	{
		if (Instance == null)
		{
            Instance = this;
        }
	}

	// Create event
	public EventBase CreateLoadingEvent(EventBase nextEvent)
	{
		EventBase created = ScriptableObject.CreateInstance<LoadingEvent>();
		created.NextEvent = nextEvent;
		return created;
	}

	// Play input event 
    public void PlayEvent(EventBase playingEvent)
	{
        // Set current progressing event to storyEvent
        CurrentEvent = playingEvent;

        switch (CurrentEvent.EventType)
		{
			case EventBase.TYPE.STORY:
				PlayStory();
				break;
			case EventBase.TYPE.LOADING:
				ShowLoading();
				break;
			case EventBase.TYPE.IN_GAME:
				PlayInGameEvent();
				break;
			case EventBase.TYPE.AUTO_SAVE:
				AutoSave();
				break;
		}
	}

	// Current event is over
	public void EventOver()
	{
		// Check if there is next event
		if(NextEvent != null)
		{
			PlayEvent(NextEvent);
		}
		else
		{
			// Reset event information
			CurrentEvent = null;

			// Show Control UI
            InputManager.Instance.ChangeState(InputManager.STATE.CONTROL, true);
        }
	}

	// Play story event
	private void PlayStory()
	{
		InputManager.Instance.ChangeState(InputManager.STATE.STORY, true); // Change UI to Story mode
		DataManager.Instance.LoadStoryText(); // Load text of the current story event
	}

	// Play InGame event
	private void PlayInGameEvent()
	{
		Debug.Log("play in game event");
	}

	// Show Loading
	private void ShowLoading()
	{
        InputManager.Instance.ChangeState(InputManager.STATE.LOADING, true); // Change UI to Loading mode
	}

	// Auto Save current player data
	private void AutoSave()
	{
		DataManager.Instance.AutoSaveUserData();
		EventOver();
	}


}

