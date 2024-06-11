using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewDataSave", menuName = "Event/DataSaveEvent", order = 0)]
public class DataSaveEvent : EventBase
{
    public int slotNum; // if 0 auto save, else save in slot

    // Set event Type on load
    public void OnEnable()
    {
        EventType = TYPE.DATA_SAVE;
    }
}
