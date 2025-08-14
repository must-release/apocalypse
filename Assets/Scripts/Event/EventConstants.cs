    public enum GameEventType
    {
        Story, Tutorial, Cutscene, MapTransition, UIChange,
        DataSave, DataLoad, SceneLoad, SceneActivate, ScreenEffect, Sequential, StageTransition, FallDeath, BGM, SFX,

        GameEventTypeCount
    };

    public enum CommonEventType
    {
        SplashScreen,
        Continue,
        NewGame,
        ReturnToTitle,
        GameOver,

        CommonEventTypeCount
    }

    public enum EventStatus
    {
        Waiting,
        Running,
        Blocked,
        Terminated,
        
        EventStatusCount
    }