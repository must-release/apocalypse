using UnityEngine;
using UIEnums;
using EventEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewSceneActivate", menuName = "Event/SceneActivateEvent", order = 0)]
public class SceneActivateEvent : EventBase
{
    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.SCENE_ACTIVATE;
    }
}
