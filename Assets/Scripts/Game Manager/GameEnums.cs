﻿using System;
using Unity.VisualScripting;

namespace UIEnums
{
    public enum BaseUI { SplashScreen, Title, Control, Story, Cutscene, Loading, GameOver, BaseUICount }
    public enum SubUI { None, Pause, Save, Load, Choice, Preference, KeySettings, Saving, SubUICount }
}

namespace StageEnums
{
    public enum STAGE { TEST, PROLOGUE, LIBRARY, EVENT }
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
        DataSave, DataLoad, SceneLoad, SceneActivation, Choice, ScreenEffect,

        GameEventTypeCount
    };

    public enum GAME_EVENT
    {
        GAME_OVER
    }
}

namespace SceneEnums
{
    public enum SCENE { TITLE, STAGE, BOSS }
}

namespace ScreenEffectEnums
{
    public enum SCREEN_EFFECT { FADE_IN, FADE_OUT }
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
        public enum BaseUIName
        {
            SplashScreenUI, TitleUI, ControlUI, StoryUI, CutsceneUI, LoadingUI, GameOverUI
        }

        public enum SubUIName
        {
            ChoiceUI, PauseUI, SaveLoadUI, PreferenceUI, KeySettingsUI, SavingUI
        }

        public static readonly string PathPrefix = "UIAssets/";
    }
}
