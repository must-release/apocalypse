using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
public class StoryBlock
{
    [XmlAttribute("BranchId")]
    public string BranchId;

    [XmlElement(typeof(StoryDialogue), ElementName = "Dialogue")]
    [XmlElement(typeof(StoryCharacterStanding), ElementName = "CharacterStanding")]
    [XmlElement(typeof(StoryChoice), ElementName = "Choice")]
    [XmlElement(typeof(StoryVFX), ElementName = "VFX")]
    public List<StoryEntry> Entries;
}