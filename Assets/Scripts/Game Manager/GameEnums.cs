using System;

namespace UIEnums
{
    public enum BASEUI { SPLASH_SCREEN, TITLE, CONTROL, STORY, CUTSCENE, LOADING }
    public enum SUBUI { NONE, PAUSE, SAVE, LOAD, CHOICE, PREFERENCE, KEYSETTINGS, SAVING }
}

namespace StageEnums
{
    public enum STAGE { TEST, PROLOGUE, LIBRARY, EVENT }
}

namespace CharacterEums
{
    public enum PLAYER { HERO, HEROINE }
    public enum PLAYER_LOWER_STATE { IDLE, RUNNING, JUMPING, AIMING, CLIMBING, PUSHING, TAGGING, DAMAGED }
    public enum PLAYER_UPPER_STATE { DISABLED, IDLE, RUNNING, JUMPING, LOOKING_UP, AIMING, ATTACKING, TOP_ATTACKING, AIM_ATTACKING }
    public enum ENEMY_STATE { PATROLLING, CHASING, ATTACKING, DAMAGED }

}

namespace WeaponEnums
{
    public enum WEAPON_TYPE { BULLET, GRANADE }
}

namespace EventEnums
{
    public enum EVENT_TYPE
    {
        STORY, TUTORIAL, CUTSCENE, MAP_CHANGE, CHANGE_UI,
        DATA_SAVE, DATA_LOAD, SCENE_LOAD, SCENE_ACTIVATE, CHOICE, SCREEN_EFFECT
    };
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
