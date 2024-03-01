using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EventManager : MonoBehaviour
{
	public static EventManager Instance { get; private set; }

	void Awake()
	{
		if (Instance == null)
			Instance = this;
	}

	public void PlayEvent(IEvent playingEvent)
	{
		switch (playingEvent.EventType)
		{
			case IEvent.TYPE.STORY:
				PlayStory((StoryEvent)playingEvent);
				break;

		}
	}

	public void EventOver()
	{
		// Reset Player's event information
		GameManager.Instance.PlayerData.currentEvent = null;
		GameManager.Instance.PlayerData.lastDialogueNum = 0;

		InputManager.Instance.ChangeState(InputManager.STATE.CONTROL);
	}

	// Play story Event
	public void PlayStory(StoryEvent storyEvent)
	{
		GameManager.Instance.PlayerData.currentEvent = storyEvent; // Modify PlayerData's event state to storyEvent
		InputManager.Instance.ChangeState(InputManager.STATE.STORY); // Change UI to Story mode
		DataManager.Instance.LoadStoryText(); // Load text of the current story event
	}
}

