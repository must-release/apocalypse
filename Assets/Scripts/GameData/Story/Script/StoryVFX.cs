using System.Xml.Serialization;

namespace AD.Story
{
    [System.Serializable]
    public class StoryVFX : StoryEntry
    {
        public enum VFXType
        {
            ScreenFadeIn,
            ScreenFadeOut
        }

        public StoryVFX()
        {
            Type = EntryType.VFX;
            IsAutoProgress = true;
        }

        [XmlAttribute("VFX")]
        public VFXType VFX;

        [XmlAttribute("Duration")]
        public float Duration;
    }
}