using System.Diagnostics;
using System.Xml.Serialization;

namespace AD.Story
{
    [System.Serializable]
    public class StoryBackgroundCG : StoryEntry
    {
        /****** Public Members ******/

        public enum BackgroundAnimationType
        {
            Appear,
            Move,
            Shake,
            ZoomIn,
            ZoomOut,
            Reset,
            BackgroundAnimtationTypeCount
        }

        public enum BackgroundPositionType
        {
            Center = 0,
            Left,
            Right,
            BackgroundPositionTypeCount
        }

        public StoryBackgroundCG()
        {
            Type = EntryType.BackgroundCG;
        }

        [XmlAttribute("Chapter")]
        public ChapterType Chapter = ChapterType.ChapterTypeCount;
        
        [XmlAttribute("ImageName")]
        public string ImageName = "";

        [XmlAttribute("Animation")]
        public BackgroundAnimationType Animation = BackgroundAnimationType.Appear;

        [XmlAttribute("AnimationDuration")]
        public float AnimationDuration = 1.0f;

        [XmlAttribute("TargetPosition")]
        public BackgroundPositionType TargetPosition = BackgroundPositionType.Center;
    }
}