using UnityEngine;
using System.Collections;
using UIEnums;
using EventEnums;
using System.Collections.Generic;

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

    // Succeed every parent event
    public void SucceedParentEvents(ref GameEvent parentEvent)
    {
        GameEvent _parentEvent = parentEvent;

        do
        {
            TerminateGameEvent(_parentEvent, true);

            _parentEvent = _parentEvent.ParentEvent;
        } while (_parentEvent);

        parentEvent = null;
    }
}

