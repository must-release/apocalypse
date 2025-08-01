using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
public class StoryChoiceOption
{
    [XmlAttribute("BranchName")]
    public string BranchName = StoryBlock.CommonBranch;

    [XmlText]
    public string Text;
}