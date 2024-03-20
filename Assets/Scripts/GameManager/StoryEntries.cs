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
    public string branchId; // For story branch
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
    public string branchId;
}

[System.Serializable]
public class StoryEntries
{
    public List<StoryEntry> entries;
}

// Used to load story info
public interface IStoryInfo
{
    public Queue<StoryEntry> StoryQueue { get; set; }
    public int LastDialogueNum { get; set; }
}