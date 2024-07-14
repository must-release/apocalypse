using UnityEngine;
using UIEnums;
using EventEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewCutscene", menuName = "Event/CutsceneEvent", order = 0)]
public class CutsceneEvent : EventBase
{
    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.CUTSCENE;
    }
}
