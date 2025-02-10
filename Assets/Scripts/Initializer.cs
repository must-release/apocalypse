using UnityEngine;

public static class Initializer
{
    // Start Main System before the main scene is loaded
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Main()
    {
        GameObject mainSystem = new GameObject("Main System");
        mainSystem.AddComponent<MainSystem>();
        Object.DontDestroyOnLoad(mainSystem);
    }
}