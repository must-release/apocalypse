using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewAutoSave", menuName = "Event/AutoSaveEvent", order = 0)]
public class AutoSaveEvent : EventBase
{
    // Set event Type on load
    public void OnEnable() { EventType = TYPE.AUTO_SAVE; }
}
