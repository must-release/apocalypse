using UnityEngine;


[CreateAssetMenu(fileName = "DevConfig", menuName = "ScriptableObjects/DevConfig", order = 1)]
public class DevConfig : ScriptableObject
{
    public SequentialEventInfo StartEvent = null;
}
