using UnityEngine;

public enum LogCategory
{
    Default,
    Event,
    GamePlay,
    AssetLoad
}

public enum LogLevel
{
    Log,
    Warning,
    Error
}

public static class Logger
{
    /// <summary>
    /// 통합 로그 출력 함수
    /// </summary>
    /// <param name="category">로그 카테고리</param>
    /// <param name="message">출력 메시지</param>
    /// <param name="level">Log / Warning / Error</param>
    /// <param name="isDebugOnly">true면 DEBUG_LOG 심볼 없으면 빌드 시 로그 제거</param>
    public static void Write(LogCategory category, string message, LogLevel level = LogLevel.Log, bool isDebugOnly = false)
    {
#if UNITY_EDITOR
        if (isDebugOnly)
        {
            Print(category, message, level);
        }
#else
        if (!isDebugOnly)
        {
            Print(category, message, level);
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
