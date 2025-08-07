using System.Xml.Serialization;

namespace AD.Story
{
    [System.Serializable]
    public class StoryBackgroundCG : StoryEntry
    {
        public StoryBackgroundCG() { }

        [XmlAttribute("Chapter")]
        public ChapterType Chapter;

        [XmlAttribute("ImageName")]
        public string ImageName;
    }
}