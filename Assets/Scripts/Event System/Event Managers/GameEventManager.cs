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
        SetRelationship(EventPointer, firstEvent);

        // play first event
        PlayGameEvent(firstEvent);
    }

	// Set parent-chile relationship
	private void SetRelationship(GameEvent parentEvent, GameEvent childEvent)
	{
		childEvent.ParentEvent = parentEvent;
	}

    // Play game event 
    private void PlayGameEvent(GameEvent playingEvent)
	{
		// Update event pointer
 		EventPointer = playingEvent;

		// Play event
		playingEvent.PlayEvent();
	}

	// Play coroutine for game events, which are scriptable objects
    public void StartCoroutineForGameEvents(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
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
            // Check compatibility of the next event
            if (!EventChecker.Instance.CheckEventCompatibility(terminatingEvent.NextEvent, terminatingEvent.ParentEvent))
            {
                yield break;
            }

            // Update event relationship. Update parent event of the child event.
            SetRelationship(terminatingEvent.ParentEvent, terminatingEvent.NextEvent);

            // Play next event
            PlayGameEvent(terminatingEvent.NextEvent);
		}
		else
		{
            // Set EventPointer to parent event
			EventPointer = terminatingEvent.ParentEvent;
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

	// Load scene
	private void LoadGameScene(SceneLoadEvent sceneLoadEvent)
	{
		// Load scene asynchronously
		GameSceneController.Instance.LoadGameScene(sceneLoadEvent.sceneName);

		//StartCoroutine(TerminateEvent(sceneLoadEvent, true, true));
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

