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
				
		}
	}

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

	// Play story Event
	public void PlayStory(StoryEvent storyEvent)
	{
		InputManager.Instance.ChangeState(InputManager.STATE.STORY); // Change UI to Story mode
		DataManager.Instance.LoadStoryText(); // Load text of the current story event
	}
}

