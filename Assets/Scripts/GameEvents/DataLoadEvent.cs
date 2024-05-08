using UnityEngine;
using System.Collections;

[System.Serializable]
[CreateAssetMenu(fileName = "NewDataLoad", menuName = "Event/DataLoadEvent", order = 0)]
public class DataLoadEvent : EventBase
{
    public int slotNum; // if -1 new game data, else if 0 continue data, else slot data

    // Set event Type on load
    public void OnEnable()
    {
        EventType = TYPE.DATA_LOAD;
    }
}

