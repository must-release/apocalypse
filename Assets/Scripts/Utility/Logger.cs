using UnityEngine;

public enum LogCategory
{
    Default,
    Event,
    GamePlay,
    GameScene,
    AssetLoad,
    StoryScriptEditor
}

public enum LogLevel
{
    Log,
    Warning,
    Error
}

public static class Logger
{
    public static void Write(LogCategory category, string message, LogLevel level = LogLevel.Log, bool isDebugOnly = false)
    {
#if UNITY_EDITOR
        Print(category, message, level);
#else
        if (!isDebugOnly)
        {
            // Save log else where if needed
        }
#endif
    }

    private static void Print(LogCategory category, string message, LogLevel level)
    {
        string prefix = $"[{category}]";

        switch (level)
        {
            case LogLevel.Warning:
                Debug.LogWarning($"{prefix} {message}");
                break;
            case LogLevel.Error:
                Debug.LogError($"{prefix} {message}");
                break;
            default:
                Debug.Log($"{prefix} {message}");
                break;
        }
    }
}
