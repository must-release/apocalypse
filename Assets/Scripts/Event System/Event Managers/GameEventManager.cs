using UnityEngine;
using System.Collections;
using UIEnums;
<<<<<<< HEAD
using EventEnums;
=======
>>>>>>> origin/minjung


public class GameEventManager : MonoBehaviour
{
	public static GameEventManager Instance { get; private set; }

<<<<<<< HEAD
	public GameEvent EventPointer { get; private set; }
=======
	public EventBase HeadEvent { get; private set; }
>>>>>>> origin/minjung

	void Awake()
	{
		if (Instance == null)
		{
            Instance = this;
        }
	}

	// Start Event Chain
<<<<<<< HEAD
	public void StartEventChain(GameEvent firstEvent)
=======
	public void StartEventChain(EventBase firstEvent)
>>>>>>> origin/minjung
	{
        // Set parent-child relationship to first event and head event
        SetRelationship(firstEvent);

        // play first event
		PlayEvent(firstEvent);
    }

    // Set parent-child relationship of the event
<<<<<<< HEAD
    private void SetRelationship(GameEvent childEvent)
    {
        // Set relationship if there is current event
        if (EventPointer)
        {
            childEvent.ParentEvent = EventPointer;
=======
    private void SetRelationship(EventBase childEvent)
    {
        // Set relationship if there is current event
        if (HeadEvent)
        {
            HeadEvent.ChildEvent = childEvent;
            childEvent.ParentEvent = HeadEvent;
>>>>>>> origin/minjung
        }
    }

    // Play event 
<<<<<<< HEAD
    private void PlayEvent(GameEvent playingEvent)
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
=======
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
>>>>>>> origin/minjung
        }
	}

	// Terminate event
<<<<<<< HEAD
	IEnumerator TerminateEvent(GameEvent terminatingEvent, bool checkChild, bool playNextEvent)
=======
	IEnumerator TerminateEvent(EventBase terminatingEvent, bool checkChild, bool playNextEvent)
>>>>>>> origin/minjung
	{
        // Wait child event chain to be terminated
        if (checkChild)
		{
<<<<<<< HEAD
			while (EventPointer != terminatingEvent)
=======
			while (terminatingEvent.ChildEvent)
>>>>>>> origin/minjung
			{
				yield return null;
			}
		}

        // Additional action
        switch (terminatingEvent.EventType)
		{
<<<<<<< HEAD
			case EVENT_TYPE.STORY:
=======
			case EventBase.TYPE.STORY:
>>>>>>> origin/minjung
				StoryController.Instance.FinishStory(); // End Story Mode
				break;
		}


        // Check if there is next event and play next
        if (terminatingEvent.NextEvent && playNextEvent)
		{
			// Update event relationship. Update child event of the parent event.
			if (terminatingEvent.ParentEvent)
			{
<<<<<<< HEAD
=======
				terminatingEvent.ParentEvent.ChildEvent = terminatingEvent.NextEvent;
>>>>>>> origin/minjung
				terminatingEvent.NextEvent.ParentEvent = terminatingEvent.ParentEvent;
			}

			PlayEvent(terminatingEvent.NextEvent);
		}
		else
		{
            // Update event relationship. Delete child event of the parent event
            if (terminatingEvent.ParentEvent)
            {
<<<<<<< HEAD
				EventPointer = terminatingEvent.ParentEvent; // Now resume the parent event chain
            }
			else
			{
				EventPointer = null; // Every event is terminated
=======
				terminatingEvent.ParentEvent.ChildEvent = null; // Child event chain is terminated
				HeadEvent = terminatingEvent.ParentEvent; // Now resume the parent event chain
            }
			else
			{
				HeadEvent = null; // Every event is terminated
>>>>>>> origin/minjung
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
<<<<<<< HEAD
	private void ShowChoice(ChoiceEvent showChoiceEvent)
=======
	private void ShowChoice(ShowChoiceEvent showChoiceEvent)
>>>>>>> origin/minjung
	{
		UIController.Instance.TurnSubUIOn(SUBUI.CHOICE);

		StartCoroutine(TerminateEvent(showChoiceEvent, true, true));
	}

<<<<<<< HEAD
=======
	// Notify selected choice option to the story system
	private void SelectChoice(SelectChoiceEvent selectChoiceEvent)
	{
		StartCoroutine(TerminateEvent(selectChoiceEvent, true, true));

		StoryController.Instance.ApplySelectedChoice(selectChoiceEvent.optionText);
    }

>>>>>>> origin/minjung
}

