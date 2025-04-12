using System.Collections.Generic;
using System.Xml.Serialization;

/******* Story Objects *******/

[XmlInclude(typeof(Dialogue))]
[XmlInclude(typeof(Effect))]
[XmlInclude(typeof(Choice))]
[System.Serializable]
public class StoryEntry
{
    [XmlIgnore]
    public bool isSavePoint = true;
}

[System.Serializable]
public class Dialogue : StoryEntry
{
    public Dialogue() { }

    public Dialogue(string character, string text)
    {
        this.character  = character;
        this.text       = text;
    }

    [XmlElement("Character")]
    public string character;

    [XmlElement("Text")]
    public string text;
}

[System.Serializable]
public class Effect : StoryEntry
{
    public Effect() { isSavePoint = false; }

    public Effect(string action, int duration)
    {
        this.action     = action;
        this.duration   = duration;
        isSavePoint     = false;
    }

    [XmlAttribute("Action")]
    public string action;

    [XmlAttribute("Duration")]
    public int duration;
}

[System.Serializable]
public class Choice : StoryEntry
{
    public Choice() { }

    public Choice(Dialogue prevDialogue, List<Option> options)
    {
        this.prevDialogue   = prevDialogue;
        this.options        = options;
    }

    [XmlElement("PrevDialogue")]
    public Dialogue prevDialogue;

    [XmlArray("Options")]
    [XmlArrayItem("Option")]
    public List<Option> options;
}

[System.Serializable]
public class Option
{
    [XmlAttribute("BranchId")]
    public string branchId;

    [XmlText]
    public string text;
}

[System.Serializable]
public class StoryBlock
{
    [XmlAttribute("BranchId")]
    public string branchId;

    [XmlElement]
    public List<StoryEntry> entries;
}

[System.Serializable]
[XmlRoot("StoryBlocks")]
public class StoryBlocks
{
    [XmlElement("Block")]
    public List<StoryBlock> blocks;
}
