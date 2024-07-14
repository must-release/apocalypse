using UnityEngine;
using System.Collections;
using UIEnums;
using EventEnums;


public class GameEventManager : MonoBehaviour
{
	public static GameEventManager Instance { get; private set; }

	public EventBase EventPointer { get; private set; }

	void Awake()
	{
		if (Instance == null)
		{
            Instance = this;
        }
	}

	// Start Event Chain
	public void StartEventChain(EventBase firstEvent)
	{
        // Set parent-child relationship to first event and head event
        SetRelationship(firstEvent);

        // play first event
		PlayEvent(firstEvent);
    }

    // Set parent-child relationship of the event
    private void SetRelationship(EventBase childEvent)
    {
        // Set relationship if there is current event
        if (EventPointer)
        {
            childEvent.ParentEvent = EventPointer;
        }
    }

    // Play event 
    private void PlayEvent(EventBase playingEvent)
	{
		// Update head event
		EventPointer = playingEvent;

        switch (EventPointer.EventType)
		{
			case EVENT_TYPE.STORY:
				StartCoroutine(PlayStory((StoryEvent)playingEvent));
				break;
			case EVENT_TYPE.CUTSCENE:
				PlayInGameEvent();
				break;
			case EVENT_TYPE.SCENE_LOAD:
				LoadGameScene((SceneLoadEvent)playingEvent);
				break;
            case EVENT_TYPE.SCENE_ACTIVATE:
                ShowLoading();
                break;
            case EVENT_TYPE.DATA_LOAD:
				LoadGameData((DataLoadEvent)playingEvent);
				break;
            case EVENT_TYPE.DATA_SAVE:
				AutoSave();
				break;
			case EVENT_TYPE.UI_CHANGE:
				ChangeUI();
				break;
        }
	}

	// Terminate event
	IEnumerator TerminateEvent(EventBase terminatingEvent, bool checkChild, bool playNextEvent)
	{
        // Wait child event chain to be terminated
        if (checkChild)
		{
			while (EventPointer != terminatingEvent)
			{
				yield return null;
			}
		}

        // Additional action
        switch (terminatingEvent.EventType)
		{
			case EVENT_TYPE.STORY:
				StoryController.Instance.FinishStory(); // End Story Mode
				break;
		}


        // Check if there is next event and play next
        if (terminatingEvent.NextEvent && playNextEvent)
		{
			// Update event relationship. Update child event of the parent event.
			if (terminatingEvent.ParentEvent)
			{
				terminatingEvent.NextEvent.ParentEvent = terminatingEvent.ParentEvent;
			}

			PlayEvent(terminatingEvent.NextEvent);
		}
		else
		{
            // Update event relationship. Delete child event of the parent event
            if (terminatingEvent.ParentEvent)
            {
				EventPointer = terminatingEvent.ParentEvent; // Now resume the parent event chain
            }
			else
			{
				EventPointer = null; // Every event is terminated
			}
        }
    }

	// Play story event
	IEnumerator PlayStory(StoryEvent storyEvent)
	{
        UIController.Instance.ChangeBaseUI(BASEUI.STORY); // Change UI to Story UI

        // Start Story according to event info
        string storyInfo = "STORY_" + storyEvent.stage.ToString() + '_' + storyEvent.storyNum;
        StoryController.Instance.StartStory(storyInfo, storyEvent.readBlockCount, storyEvent.readEntryCount);

		// Wait for story to end
		while (StoryController.Instance.IsStoryPlaying)
		{
			yield return null;
		}

		// Terminate story event
		StartCoroutine(TerminateEvent(storyEvent, true, true));
	}

	// Play InGame event
	private void PlayInGameEvent()
	{
        //UIController.Instance.ChangeState(UIController.STATE.EMPTY, true); // Empty UI
        Debug.Log("play in game event");
		//TerminateEvent();
	}

	// Show Loading
	private void ShowLoading()
	{
        //UIController.Instance.ChangeState(UIController.STATE.LOADING, true); // Change UI to Loading mode
	}

	// Load scene
	private void LoadGameScene(SceneLoadEvent sceneLoadEvent)
	{
		// Load scene asynchronously
		GameSceneController.Instance.LoadGameScene(sceneLoadEvent.sceneName);

		StartCoroutine(TerminateEvent(sceneLoadEvent, true, true));
	}


	// Load game data from local, or create new game data
	private void LoadGameData(DataLoadEvent dataLoadEvent)
	{
		if(dataLoadEvent.isNewGame) // create new game data
		{
			DataManager.Instance.CreateNewGameData();
		}
		else
		{
			
		}


		// Terminate data load event and play next event
		StartCoroutine(TerminateEvent(dataLoadEvent, true, true));
	}

	// Auto Save current player data
	private void AutoSave()
	{
		DataManager.Instance.AutoSaveUserData();
		//TerminateEvent();
	}

	// Change UI state
	private void ChangeUI()
	{
		//UIController.STATE ui = HeadEvent.GetEventInfo<UIController.STATE>();
		//UIController.Instance.ChangeUI(ui);
		//TerminateEvent();
	}

	// Show Choice UI
	private void ShowChoice(ChoiceEvent showChoiceEvent)
	{
		UIController.Instance.TurnSubUIOn(SUBUI.CHOICE);

		StartCoroutine(TerminateEvent(showChoiceEvent, true, true));
	}

}

