using System.Collections.Generic;
using System.Xml.Serialization;

namespace AD.Story
{
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
        [XmlElement(typeof(StoryCharacterCG), ElementName = "CharacterCG")]
        [XmlElement(typeof(StoryChoice), ElementName = "Choice")]
        [XmlElement(typeof(StoryBackgroundCG), ElementName = "BackgroundCG")]
        [XmlElement(typeof(StoryBGM), ElementName = "BGM")]
        [XmlElement(typeof(StoryCameraAction), ElementName = "CameraAction")]
        [XmlElement(typeof(StoryPlayMode), ElementName = "PlayMode")]
        [XmlElement(typeof(StorySFX), ElementName = "SFX")]
        [XmlElement(typeof(StoryVFX), ElementName = "VFX")]
        public List<StoryEntry> Entries;
    }
}