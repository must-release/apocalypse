using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
public class StoryBlock
{
    public const string CommonBranch = "Common";
    
    public bool IsCommon => IsCommonBranch(BranchName);
    
    public static bool IsCommonBranch(string branchName)
    {
        return string.Equals(branchName, CommonBranch, System.StringComparison.OrdinalIgnoreCase);
    }

    [XmlAttribute("BranchName")]
    public string BranchName = CommonBranch;

    [XmlElement(typeof(StoryDialogue), ElementName = "Dialogue")]
    [XmlElement(typeof(StoryCharacterStanding), ElementName = "CharacterStanding")]
    [XmlElement(typeof(StoryChoice), ElementName = "Choice")]
    [XmlElement(typeof(StoryVFX), ElementName = "VFX")]
    public List<StoryEntry> Entries;
}