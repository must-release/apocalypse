using System.Xml.Serialization;

[System.Serializable]
public class StoryDialogue : StoryEntry
{
    public enum TextSpeedType
    {
        Default = 0,
        Slow,
        Fast
    }

    public StoryDialogue() { }

    public StoryDialogue(string name, string text)
    {
        Name        = name;
        Text        = text;
        IsSavePoint = true;
    }

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("Speed")]
    public TextSpeedType TextSpeed;

    [XmlText]
    public string Text;
}