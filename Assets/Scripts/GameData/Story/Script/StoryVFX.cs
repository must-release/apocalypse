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

        public StoryVFX() { }

        [XmlAttribute("VFX")]
        public VFXType VFX;

        [XmlAttribute("Duration")]
        public float Duration;
    }
}