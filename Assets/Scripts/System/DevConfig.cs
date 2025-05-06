using UnityEngine;


[CreateAssetMenu(fileName = "DevConfig", menuName = "ScriptableObjects/DevConfig", order = 1)]
public class DevConfig : ScriptableObject
{
    public static string AssetPath => "Assets/GameResources/Dev/DevConfig";

    public bool ShowSplashScreen = true;

    public SequentialEventInfo StartEvent = null;
}
