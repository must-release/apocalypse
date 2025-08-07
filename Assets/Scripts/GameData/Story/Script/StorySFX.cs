using System.Xml.Serialization;

namespace AD.Story
{
    [System.Serializable]
    public class StorySFX : StoryEntry
    {
        public StorySFX() { }

        [XmlAttribute("SFXName")]
        public string SFXName;
    }
}