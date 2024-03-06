using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EventManager : MonoBehaviour
{
	public static EventManager Instance { get; private set; }

	public IEvent CurrentEvent { get; private set; }

	void Awake()
	{
		if (Instance == null)
			Instance = this;
	}

	public void PlayEvent(IEvent playingEvent)
	{
        // Set current progressing event to storyEvent
        CurrentEvent = playingEvent;

        switch (CurrentEvent.EventType)
		{
			case IEvent.TYPE.STORY:
				PlayStory((StoryEvent)CurrentEvent);
				break;
			case IEvent.TYPE.LOADING:
				ShowLoading();
				break;
			case IEvent.TYPE.IN_GAME:
				PlayInGameEvent((InGameEvent)playingEvent);
				break;
			case IEvent.TYPE.AUTO_SAVE:
				AutoSave();
				break;
		}
	}

	// Current event is over
	public void EventOver()
	{
		// Check if there is next event
		if(CurrentEvent.NextEvent != null)
		{
			PlayEvent(CurrentEvent.NextEvent);
		}
		else
		{
			// Reset event information
			CurrentEvent = null;

			// Show Control UI
            InputManager.Instance.ChangeState(InputManager.STATE.CONTROL);
        }
	}

	// Play story event
	public void PlayStory(StoryEvent storyEvent)
	{
		InputManager.Instance.ChangeState(InputManager.STATE.STORY); // Change UI to Story mode
		DataManager.Instance.LoadStoryText(); // Load text of the current story event
	}

	// Play InGame event
	public void PlayInGameEvent(InGameEvent inGameEvent)
	{
		Debug.Log("play in game event");
	}

	// Show Loading
	public void ShowLoading()
	{
		InputManager.Instance.ChangeState(InputManager.STATE.LOADING); // Change UI to Loading mode
	}

	// Auto Save current player data
	public void AutoSave()
	{
		DataManager.Instance.AutoSave();
		EventOver();
	}
}

