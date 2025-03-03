﻿using UnityEngine;
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

    private Coroutine choiceCoroutine;

    // Set event Type on load
    public void OnEnable()
    {
        EventType = EventEnums.GameEventType.Choice;
    }

    // Check compatibility with current event
    public override bool CheckCompatibility(GameEvent parentEvent, BaseUI baseUI, SubUI subUI)
    {
        if (parentEvent.EventType == EventEnums.GameEventType.Story) // Can be played when story event is playing
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
        choiceCoroutine = GameEventManager.Instance.StartCoroutine(PlayEventCoroutine());
    }
    public override IEnumerator PlayEventCoroutine()
    {
        // Set choice info and switch to choice UI
        UIController.Instance.SetChoiceInfo(choiceList);
        UIController.Instance.TurnSubUIOn(SubUI.Choice);

        // Wait for player to select a choice
        while (UIController.Instance.GetSelectedChoice() == null)
        {
            yield return null;
        }

        // Get the selected choice and process it
        string selectedChoice = UIController.Instance.GetSelectedChoice();
        bool generateResponse = choiceList == null;
        StoryController.Instance.ProcessSelectedChoice(selectedChoice, generateResponse);

        // Terminate the choice event
        GameEventManager.Instance.TerminateGameEvent(this);
    }

    // Terminate choice event
    public override void TerminateEvent()
    {
        GameEventManager.Instance.StopCoroutine(choiceCoroutine);
    }
}