using System.Collections.Generic;
using System.Xml.Serialization;

namespace AD.Story
{
    [System.Serializable]
    public class StoryChoiceOption
    {
        [XmlAttribute("BranchName")]
        public string BranchName = StoryBlock.CommonBranch;

        [XmlText]
        public string Text;
    }
}