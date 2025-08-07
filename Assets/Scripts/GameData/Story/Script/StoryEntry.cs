using System.Collections.Generic;
using System.Xml.Serialization;

[XmlInclude(typeof(StoryDialogue))]
[XmlInclude(typeof(StoryCharacterCG))]
[XmlInclude(typeof(StoryChoice))]
[XmlInclude(typeof(StoryBackgroundCG))]
[XmlInclude(typeof(StoryBGM))]
[XmlInclude(typeof(StoryCameraAction))]
[XmlInclude(typeof(StoryPlayMode))]
[XmlInclude(typeof(StorySFX))]
[XmlInclude(typeof(StoryVFX))]
[System.Serializable]
public class StoryEntry
{
    [XmlIgnore]
    public bool IsSavePoint = false;
}