using System.Collections.Generic;
using System.Xml.Serialization;

namespace AD.Story
{
    [System.Serializable]
    public class StoryChoice : StoryEntry
    {
        public StoryChoice() 
        {
            Type = EntryType.Choice;
        }

        public StoryChoice(StoryDialogue prevDialogue, List<StoryChoiceOption> options)
        {
            Type = EntryType.Choice;
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
}