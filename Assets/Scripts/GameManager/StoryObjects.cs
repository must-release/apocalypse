using System.Collections.Generic;

/******* Story Objects *******/

[System.Serializable]
public class StoryEntry
{
    public string type;
    public bool savePoint = true;
}

[System.Serializable]
public class Dialogue : StoryEntry
{
    public string character;
    public string text;
}

[System.Serializable]
public class Effect : StoryEntry
{
    public string action;
    public int duration;
}

[System.Serializable]
public class Choice : StoryEntry
{
    public Dialogue prevDialogue;
    public List<Option> options;
}

[System.Serializable]
public class Option
{
    public string text;
    public string branchId;
}

[System.Serializable]
public class StoryBlock
{
    public string branchId;
    public List<StoryEntry> entries;
}

[System.Serializable]
public class StoryBlocks
{
    public List<StoryBlock> blocks;
}