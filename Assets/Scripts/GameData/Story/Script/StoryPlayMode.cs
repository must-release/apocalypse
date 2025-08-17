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
        }

        public StoryPlayMode(PlayModeType playMode)
        {
            Debug.Assert(playMode != PlayModeType.PlayModeTypeCount, "Invalid PlayMode type: " + playMode);

            Type = EntryType.PlayMode;
            PlayMode = playMode;
        }

        [XmlAttribute("Mode")]
        public PlayModeType PlayMode;
    }
}