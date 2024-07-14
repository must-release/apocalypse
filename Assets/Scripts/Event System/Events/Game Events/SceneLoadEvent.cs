using UnityEngine;
using UIEnums;
using EventEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewSceneLoad", menuName = "Event/SceneLoadEvent", order = 0)]
public class SceneLoadEvent : GameEvent
{
    public string sceneName; // if null, load scene according to user data

    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.SCENE_LOAD;
    }
}

