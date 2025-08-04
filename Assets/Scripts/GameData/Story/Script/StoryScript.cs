using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
[XmlRoot("StoryScript")]
public class StoryScript
{
    [XmlElement("Block")]
    public List<StoryBlock> Blocks;
}