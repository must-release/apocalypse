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
    public enum PlayerType { Hero, Heroine, PlayerCount }
    public enum HeroLowerState { Idle, Running, Jumping, Aiming, Climbing, Pushing, Tagging, Damaged, Dead, HeroLowerStateCount }
    public enum HeroUpperState { Disabled, Idle, Running, Jumping, LookingUp, Aiming, Attacking, TopAttacking, AimAttacking, HeroUpperStateCount }
    public enum HeroineLowerState { Idle, Running, Jumping, Aiming, Climbing, Pushing, Tagging, Damaged, Dead, HeroineLowerStateCount }
    public enum HeroineUpperState { Disabled, Idle, Running, Jumping, LookingUp, Aiming, Attacking, TopAttacking, AimAttacking, HeroineUpperStateCount }
    public enum EnemyState { Patrolling, Chasing, Attacking, Damaged, Dead, EnemyStateCount }

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
    public enum SceneName { SplashScreenScene, TitleScene, StageScene, BossScene, SceneNameCount }
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
        public static readonly string PathPrefix = "Systems/";

        public enum AssetName
        {
            // Don't change the sequence
            GameManager, 
            AISystem, StorySystem, UISystem, UtilitySystem, Cameras, GamePlaySystem, GameSceneSystem,
            EventSystem
        }
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