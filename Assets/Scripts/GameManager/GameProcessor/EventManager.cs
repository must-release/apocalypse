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

	public void PlayStory(StoryEvent storyEvent)
	{
		InputManager.Instance.ChangeState(InputManager.STATE.STORY);
		DataManager.Instance.LoadStoryText(storyEvent.stageNum, storyEvent.storyNum);
	}
}

