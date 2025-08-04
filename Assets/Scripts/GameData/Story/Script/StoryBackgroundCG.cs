using System.Xml.Serialization;

[System.Serializable]
public class StoryBackgroundCG : StoryEntry
{
    public StoryBackgroundCG() { }

    [XmlAttribute("Chapter")]
    public ChapterType Chapter;

    [XmlAttribute("ImageName")]
    public string ImageName;
}