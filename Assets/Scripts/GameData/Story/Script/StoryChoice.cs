using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
public class StoryChoice : StoryEntry
{
    public StoryChoice() { }

    public StoryChoice(StoryDialogue prevDialogue, List<StoryChoiceOption> options)
    {
        PrevDialogue   = prevDialogue;
        Options        = options;
        IsSavePoint    = true;
    }

    [XmlElement("PrevDialogue")]
    public StoryDialogue PrevDialogue;

    [XmlArray("Options")]
    [XmlArrayItem("Option")]
    public List<StoryChoiceOption> Options;
}