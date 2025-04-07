using System;
using Unity.VisualScripting;

namespace UIEnums
{
    public enum BaseUI { SplashScreen, Title, Control, Story, Cutscene, Loading, GameOver, BaseUICount }
    public enum SubUI { None, Pause, Save, Load, Choice, Preference, KeySettings, Saving, SubUICount }
}

namespace StageEnums
{
    public enum Stage { Test, Prologue, Library, Event, StageCount }
}

namespace CharacterEums
{
    public enum PLAYER { HERO, HEROINE }
    public enum PLAYER_LOWER_STATE { IDLE, RUNNING, JUMPING, AIMING, CLIMBING, PUSHING, TAGGING, DAMAGED, DEAD, PLAYER_LOWER_STATE_COUNT }
    public enum PLAYER_UPPER_STATE { DISABLED, IDLE, RUNNING, JUMPING, LOOKING_UP, AIMING, ATTACKING, TOP_ATTACKING, AIM_ATTACKING }
    public enum ENEMY_STATE { PATROLLING, CHASING, ATTACKING, DAMAGED, DEAD }

}

namespace WeaponEnums
{
    public enum WEAPON_TYPE 
    { 
        // Player weapon
        BULLET, GRANADE, 

        // Monster weapon
        SCRATCH,    
        
        // Enum count
        WEAPON_TYPE_COUNT 
        
    }
}

namespace EventEnums
{
    public enum GameEventType
    {
        Story, Tutorial, Cutscene, MapTransition, UIChange,
        DataSave, DataLoad, SceneLoad, SceneActivate, Choice, ScreenEffect, Sequential,

        GameEventTypeCount
    };

    public enum GAME_EVENT
    {
        GAME_OVER
    }

    public enum EventStatus
    {
        Waiting,
        Running,
        Blocked,
        Terminated,
        
        EventStatusCount
    }

}

namespace SceneEnums
{
    public enum Scene { Title, Stage, Boss, SceneCount }
}

namespace ScreenEffectEnums
{
    public enum ScreenEffect { FadeIn, FadeOut, ScreenEffectCount }
}

namespace LayerEnums
{
    public static class LAYER
    {
        public const string GROUND = "Ground";
        public const string CHARACTER = "Character";
        public const string WEAPON = "Weapon";

    }
}

namespace AssetEnums
{
    public static class SystemAsset
    {
        public enum AssetName
        {
            // Don't change the sequence
            GameManager, 
            AISystem, StorySystem, UISystem, UtilitySystem, Cameras, GamePlaySystem, GameSceneSystem,
            EventSystem
        }

        public static readonly string PathPrefix = "Systems/";
    }

    public static class UIAsset
    {
        public static readonly string PathPrefix = "UIAssets/";

        public enum BaseUIName
        {
            SplashScreenUI, TitleUI, ControlUI, StoryUI, CutsceneUI, LoadingUI, GameOverUI
        }

        public enum SubUIName
        {
            ChoiceUI, PauseUI, SaveLoadUI, PreferenceUI, KeySettingsUI, SavingUI
        }
    }

    public static class SequentialEventInfoAsset
    {
        public static class Common
        {
            public const string NewGame         = "Common/NewGameEventInfo";
            public const string GameOver        = "Common/GameOverEventInfo";
            public const string SplashScreen    = "Common/SplashScreenInfo";
            public const string GameStart       = "Common/GameStartEventInfo";
            public const string ContinueGame    = "Common/ContinueGameEventInfo";
            public const string ReturnToTitle   = "Common/ReturnToTitleEventInfo";
        }

        public static class Prologue
        {
            public const string Trigger = "Prologue/TriggerInfo";
        }
    }
}