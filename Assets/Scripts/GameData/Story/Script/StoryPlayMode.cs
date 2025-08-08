using System.Xml.Serialization;

namespace AD.Story
{
    [System.Serializable]
    public class StoryPlayMode : StoryEntry
    {
        public enum PlayModeType
        {
            VisualNovel,
            SideDialogue,
            InGameCutScene
        }

        public StoryPlayMode() 
        {
            Type = EntryType.PlayMode;
        }

        [XmlAttribute("Mode")]
        public PlayModeType PlayMode;
    }
}