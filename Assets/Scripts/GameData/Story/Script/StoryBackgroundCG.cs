using System.Xml.Serialization;

namespace AD.Story
{
    [System.Serializable]
    public class StoryBackgroundCG : StoryEntry
    {
        public StoryBackgroundCG() 
        {
            Type = EntryType.BackgroundCG;
        }

        [XmlAttribute("Chapter")]
        public ChapterType Chapter;

        [XmlAttribute("ImageName")]
        public string ImageName;
    }
}