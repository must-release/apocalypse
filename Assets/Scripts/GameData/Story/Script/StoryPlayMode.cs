using System.Xml.Serialization;
using UnityEngine;

namespace AD.Story
{
    [System.Serializable]
    public class StoryPlayMode : StoryEntry
    {
        public enum PlayModeType
        {
            VisualNovel,
            SideDialogue,
            InGameCutScene,
            PlayModeTypeCount
        }

        public StoryPlayMode()
        {
            Type = EntryType.PlayMode;
            IsAutoProgress = true;
        }

        public StoryPlayMode(PlayModeType playMode) : this()
        {
            Debug.Assert(playMode != PlayModeType.PlayModeTypeCount, "Invalid PlayMode type: " + playMode);

            PlayMode = playMode;
        }

        [XmlAttribute("Mode")]
        public PlayModeType PlayMode;
    }
}