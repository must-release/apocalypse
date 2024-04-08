using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewUIChange", menuName = "Event/UIChange", order = 0)]
public class UIChangeEvent : EventBase
{
    public UIManager.STATE changingUI;

    // Set event Type on load
    public void OnEnable()
    {
        EventType = TYPE.UI_CHANGE;
    }

    public override T GetEventInfo<T>()
    {
        // Check generic type
        if (typeof(T) == typeof(UIManager.STATE))
        {
            // Return UI state
            object ui = changingUI;
            return (T)ui;
        }
        return default(T);
    }
}