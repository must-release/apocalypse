using System.Xml.Serialization;


[System.Serializable]
public class StorySFX : StoryEntry
{
    public StorySFX() { }

    [XmlAttribute("SFXName")]
    public string SFXName;
}