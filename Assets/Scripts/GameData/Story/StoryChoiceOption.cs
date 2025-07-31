using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
public class StoryChoiceOption
{
    [XmlAttribute("BranchId")]
    public string BranchId;

    [XmlText]
    public string Text;
}