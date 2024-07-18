using UnityEngine;
using System.Collections;
using UIEnums;
using EventEnums;


public class GameEventManager : MonoBehaviour
{
	public static GameEventManager Instance { get; private set; }

	public GameEvent EventPointer { get; private set; }

	void Awake()
	{
		if (Instance == null)
		{
            Instance = this;
        }
	}

	// Start Event Chain
	public void StartEventChain(GameEvent firstEvent)
	{
        // Set parent-child relationship to first event and pointed event
        if (EventPointer)
        {
            firstEvent.ParentEvent = EventPointer;
        }

        // play first event
        PlayGameEvent(firstEvent);
    }

    // Play game event 
    private void PlayGameEvent(GameEvent playingEvent)
	{
		// Update event pointer
 		EventPointer = playingEvent;

		// Play event
		playingEvent.PlayEvent();
	}


    // Terminate target game event
    public void TerminateGameEvent(GameEvent terminatingEvent, bool succeeding = false)
	{
		StartCoroutine(AsyncTerminateEvent(terminatingEvent, succeeding));
	}
	IEnumerator AsyncTerminateEvent(GameEvent terminatingEvent, bool succeeding)
	{
		// Check if succeeding is on progress
        if (succeeding)
		{
            // Terminate target game event
            terminatingEvent.TerminateEvent();

			// End this coroutine
			yield break;
        }
        else // Wait child event chain to be terminated
        {
            while (EventPointer != terminatingEvent)
            {
                yield return null;
            }

            // Terminate target game event
            terminatingEvent.TerminateEvent();
        }


        // Check if there is next event
        if (terminatingEvent.NextEvent)
		{
			// Update event relationship. Update parent event of the child event.
			if (terminatingEvent.ParentEvent)
			{
				terminatingEvent.NextEvent.ParentEvent = terminatingEvent.ParentEvent;
			}

			PlayGameEvent(terminatingEvent.NextEvent);
		}
		else
		{
            // Update event relationship.Set EventPointer to parent event
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
		//StartCoroutine(TerminateEvent(storyEvent, true, true));
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

		//StartCoroutine(TerminateEvent(sceneLoadEvent, true, true));
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

		//StartCoroutine(TerminateEvent(showChoiceEvent, true, true));
	}
}

