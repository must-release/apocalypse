﻿using UnityEngine;
using System.Collections;

[System.Serializable]
[CreateAssetMenu(fileName = "NewDataLoad", menuName = "Event/DataLoadEvent", order = 0)]
public class DataLoadEvent : EventBase
{
    public int slotNum; // Number of the data slot to load data
    public bool isNewGame = false; // If true, create new game data
    public bool isContinueGame = false; // If true, load most recent saved data

    // Set event Type on load
    public void OnEnable()
    {
        EventType = TYPE.DATA_LOAD;
    }

    // Check compatibility with current event
    public override bool CheckCompatibility(EventBase headEvent)
    {
        if(headEvent == null) // Can be played when there is no event playing
        {
            return true;
        }
        else if(headEvent.EventType == TYPE.STORY) // Can be played when story event is playing
        {
            return true;
        }
        else
            return false;
    }
}
