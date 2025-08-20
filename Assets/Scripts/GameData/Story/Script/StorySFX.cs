using System.Xml.Serialization;

namespace AD.Story
{
    [System.Serializable]
    public class StorySFX : StoryEntry
    {
        public StorySFX()
        {
            Type = EntryType.SFX;
            IsAutoProgress = true;
        }

        [XmlAttribute("SFXName")]
        public string SFXName;
    }
}