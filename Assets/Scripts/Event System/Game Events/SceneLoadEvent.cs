using UnityEngine;
using System.Collections;

[System.Serializable]
[CreateAssetMenu(fileName = "NewSceneLoad", menuName = "Event/SceneLoadEvent", order = 0)]
public class SceneLoadEvent : EventBase
{
    public string sceneName; // if null, load scene according to user data

    // Set event Type on load
    public void OnEnable()
    {
        EventType = TYPE.SCENE_LOAD;
    }
}

