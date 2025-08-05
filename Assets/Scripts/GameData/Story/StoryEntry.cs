using System.Collections.Generic;
using System.Xml.Serialization;

[XmlInclude(typeof(StoryDialogue))]
[XmlInclude(typeof(StoryVFX))]
[XmlInclude(typeof(StoryChoice))]
[XmlInclude(typeof(StoryCharacterStanding))]
[System.Serializable]
public class StoryEntry
{
    [XmlIgnore]
    public bool IsSavePoint = false;
}