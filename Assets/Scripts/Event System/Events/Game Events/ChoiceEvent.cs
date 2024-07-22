using UnityEngine;
using System.Collections.Generic;
using UIEnums;
using EventEnums;
using System.Collections;

[System.Serializable]
[CreateAssetMenu(fileName = "NewChoice", menuName = "Event/ChoiceEvent", order = 0)]
public class ChoiceEvent : GameEvent
{
    public List<string> choiceList;
    public string selectedChoice;


    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.CHOICE;
    }

    // Check compatibility with current event
    public override bool CheckCompatibility(GameEvent parentEvent, (BASEUI, SUBUI) currentUI)
    {
        if (parentEvent.EventType == EVENT_TYPE.STORY) // Can be played when story event is playing
        {
            return true;
        }
        else
            return false;
    }

    // Play Choice event
    public override void PlayEvent()
    {
        // Use GameEventManger to start coroutine
        GameEventManager.Instance.StartCoroutineForGameEvents(PlayEventCoroutine());
    }
    IEnumerator PlayEventCoroutine()
    {
        // Set choice info and turn UI to choice UI
        UIController.Instance.SetChoiceInfo(choiceList);
        UIController.Instance.TurnSubUIOn(SUBUI.CHOICE);

        // Wait while current UI is Choice UI
        while (UIController.Instance.GetCurrentUI().Item2 == SUBUI.CHOICE)
        {
            yield return null;
        }

        // Get selected choice and process it
        selectedChoice = UIController.Instance.GetSelectedChoice();
        StoryController.Instance.ProcessSelectedChoice(selectedChoice);

        // Terminate choice event
        GameEventManager.Instance.TerminateGameEvent(this);
    }
}