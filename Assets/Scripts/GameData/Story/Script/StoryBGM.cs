using System.Xml.Serialization;

[System.Serializable]
public class StoryBGM : StoryEntry
{
    public enum BGMAction
    {
        Start,
        Stop
    }

    public StoryBGM() { }

    [XmlAttribute("Action")]
    public BGMAction Action;

    [XmlAttribute("BGMName")]
    public string BGMName;

    [XmlAttribute("FadeDuration")]
    public float FadeDuration;

    [XmlAttribute("IsLoop")]
    public bool IsLoop;
}