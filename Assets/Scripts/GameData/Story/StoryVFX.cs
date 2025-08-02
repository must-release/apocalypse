using System.Xml.Serialization;


[System.Serializable]
public class StoryVFX : StoryEntry
{
    public StoryVFX() { IsSavePoint = false; }

    public StoryVFX(string action, float duration)
    {
        Action      = action;
        Duration    = duration;
    }

    [XmlAttribute("Action")]
    public string Action;

    [XmlAttribute("Duration")]
    public float Duration;
}