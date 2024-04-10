using System.Collections.Generic;

/******* Story Objects *******/

[System.Serializable]
public class StoryEntry
{
    public string type;
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
    public List<Option> options;
}

[System.Serializable]
public class Option
{
    public string text;
    public string routeId;
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