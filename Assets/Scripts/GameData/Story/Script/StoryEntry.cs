using System.Collections.Generic;
using System.Xml.Serialization;

namespace AD.Story
{
    [XmlInclude(typeof(StoryDialogue))]
    [XmlInclude(typeof(StoryCharacterCG))]
    [XmlInclude(typeof(StoryChoice))]
    [XmlInclude(typeof(StoryBackgroundCG))]
    [XmlInclude(typeof(StoryBGM))]
    [XmlInclude(typeof(StoryCameraAction))]
    [XmlInclude(typeof(StoryPlayMode))]
    [XmlInclude(typeof(StorySFX))]
    [XmlInclude(typeof(StoryVFX))]
    [System.Serializable]
    public class StoryEntry
    {
        public enum EntryType
        {
            Dialogue,
            VFX,
            Choice,
            CharacterCG,
            PlayMode,
            BackgroundCG,
            BGM,
            SFX,
            CameraAction
        }

        [XmlIgnore] public bool IsAutoProgress = false;
        [XmlIgnore] public bool IsSavePoint = false;
        [XmlIgnore] public EntryType Type;
    }
}