using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewCutscene", menuName = "Event/CutsceneEvent", order = 0)]
public class CutsceneEvent : EventBase
{
    // Set event Type on load
    public void OnEnable()
    {
        EventType = TYPE.CUTSCENE;
    }
}
