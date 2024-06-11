using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewSceneActivate", menuName = "Event/SceneActivateEvent", order = 0)]
public class SceneActivateEvent : EventBase
{
    // Set event Type on load
    public void OnEnable()
    {
        EventType = TYPE.SCENE_ACTIVATE;
    }
}
