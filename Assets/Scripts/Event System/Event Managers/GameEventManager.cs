using UnityEngine;
using System.Collections;
using UIEnums;


public class GameEventManager : MonoBehaviour
{
	public static GameEventManager Instance { get; private set; }

	public EventBase HeadEvent { get; private set; }

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
        if (HeadEvent)
        {
            HeadEvent.ChildEvent = childEvent;
            childEvent.ParentEvent = HeadEvent;
        }
    }

    // Play event 
    private void PlayEvent(EventBase playingEvent)
	{
		// Update head event
		HeadEvent = playingEvent;

        switch (HeadEvent.EventType)
		{
			case EventBase.TYPE.STORY:
				StartCoroutine(PlayStory((StoryEvent)playingEvent));
				break;
			case EventBase.TYPE.CUTSCENE:
				PlayInGameEvent();
				break;
			case EventBase.TYPE.SCENE_LOAD:
				LoadGameScene((SceneLoadEvent)playingEvent);
				break;
            case EventBase.TYPE.SCENE_ACTIVATE:
                ShowLoading();
                break;
            case EventBase.TYPE.DATA_LOAD:
				LoadGameData((DataLoadEvent)playingEvent);
				break;
            case EventBase.TYPE.DATA_SAVE:
				AutoSave();
				break;
			case EventBase.TYPE.UI_CHANGE:
				ChangeUI();
				break;
			case EventBase.TYPE.SHOW_CHOICE:
				ShowChoice((ShowChoiceEvent)playingEvent);
				break;
			case EventBase.TYPE.SELECT_CHOICE:
				SelectChoice((SelectChoiceEvent)playingEvent);
				break;
        }
	}

	// Terminate event
	IEnumerator TerminateEvent(EventBase terminatingEvent, bool checkChild, bool playNextEvent)
	{
        // Wait child event chain to be terminated
        if (checkChild)
		{
			while (terminatingEvent.ChildEvent)
			{
				yield return null;
			}
		}

        // Additional action
        switch (terminatingEvent.EventType)
		{
			case EventBase.TYPE.STORY:
				StoryController.Instance.FinishStory(); // End Story Mode
				break;
		}


        // Check if there is next event and play next
        if (terminatingEvent.NextEvent && playNextEvent)
		{
			// Update event relationship. Update child event of the parent event.
			if (terminatingEvent.ParentEvent)
			{
				terminatingEvent.ParentEvent.ChildEvent = terminatingEvent.NextEvent;
				terminatingEvent.NextEvent.ParentEvent = terminatingEvent.ParentEvent;
			}

			PlayEvent(terminatingEvent.NextEvent);
		}
		else
		{
            // Update event relationship. Delete child event of the parent event
            if (terminatingEvent.ParentEvent)
            {
				terminatingEvent.ParentEvent.ChildEvent = null; // Child event chain is terminated
				HeadEvent = terminatingEvent.ParentEvent; // Now resume the parent event chain
            }
			else
			{
				HeadEvent = null; // Every event is terminated
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
	private void ShowChoice(ShowChoiceEvent showChoiceEvent)
	{
		UIController.Instance.TurnSubUIOn(SUBUI.CHOICE);

		StartCoroutine(TerminateEvent(showChoiceEvent, true, true));
	}

	// Notify selected choice option to the story system
	private void SelectChoice(SelectChoiceEvent selectChoiceEvent)
	{
		StartCoroutine(TerminateEvent(selectChoiceEvent, true, true));

		StoryController.Instance.ApplySelectedChoice(selectChoiceEvent.optionText);
    }

}

