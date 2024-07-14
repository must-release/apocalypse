using System;

namespace UIEnums
{
    public enum BASEUI { TITLE, CONTROL, STORY, CUTSCENE, LOADING }
    public enum SUBUI { NONE, PAUSE, SAVE, LOAD, CHOICE, PREFERENCE, KEYSETTINGS, SAVING }
}

namespace StageEnums
{
    public enum STAGE { TEST, PROLOGUE, LIBRARY, EVENT }
}

namespace CharacterEums
{
    public enum CHARACTER { HERO, HEROINE }
}

namespace EventEnums
{
    public enum EVENT_TYPE
    {
        STORY, TUTORIAL, CUTSCENE, MAP_CHANGE, UI_CHANGE,
        DATA_SAVE, DATA_LOAD, SCENE_LOAD, SCENE_ACTIVATE, CHOICE
    };
}
