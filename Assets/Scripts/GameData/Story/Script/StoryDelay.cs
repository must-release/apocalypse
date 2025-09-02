using System.Xml.Serialization;

namespace AD.Story
{
    [System.Serializable]
    public class StoryDelay : StoryEntry
    {
        public StoryDelay()
        {
            Type = EntryType.Delay;
            IsAutoProgress = true;
            IsSingleExecuted = true;
        }

        [XmlAttribute("Duration")]
        public float Duration;
    }
}
