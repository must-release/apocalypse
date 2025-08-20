using System.Xml.Serialization;

namespace AD.Story
{
    [System.Serializable]
    public class StoryDialogue : StoryEntry
    {
        public enum TextSpeedType
        {
            Default = 0,
            Slow,
            Fast,
            TextSpeedTypeCount
        }

        public StoryDialogue()
        {
            Type = EntryType.Dialogue;
            IsSavePoint = true;
            IsAutoSkip = false;
        }

        public StoryDialogue(string name, string text) : this()
        {
            Name = name;
            Text = text;
        }

        [XmlAttribute("Name")]
        public string Name;

        [XmlAttribute("Speed")]
        public TextSpeedType TextSpeed;

        [XmlAttribute("IsAutoSkip")]
        public bool IsAutoSkip;

        [XmlText]
        public string Text = "";
    }
}