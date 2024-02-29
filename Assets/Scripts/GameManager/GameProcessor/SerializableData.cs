using System.Collections.Generic;

[System.Serializable]
public class UserData
{
    public enum STAGE { TEST, LIBRARY }

    public STAGE currentStage;
    public int currentMap;
    public IEvent currentEvent;
    public int lastDialogueNum;
    public int playTime;
    public string saveTime;

    public UserData(STAGE curStage, int curMap, IEvent curEvent, int lastDlg, int playTime, string saveTime)
    {
        currentStage = curStage;
        currentMap = curMap;
        currentEvent = curEvent;
        lastDialogueNum = lastDlg;
        this.playTime = playTime;
        this.saveTime = saveTime;
    }
}


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
