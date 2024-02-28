using System.Collections.Generic;

[System.Serializable]
public class UserData
{
    public int currentStage;
    public int currentMap;
    public IEvent currentEvent;
    public int lastDialogueNum;

    public UserData(int curStage, int curMap, IEvent curEvent, int lastDlg)
    {
        currentStage = curStage;
        currentMap = curMap;
        currentEvent = curEvent;
        lastDialogueNum = lastDlg;
    }
}

[System.Serializable]
public class DialogueEntry
{
    public string type;
    public string character;
    public string text;
    public string action;
    public float duration;
    public List<Choice> options;
}

[System.Serializable]
public class Choice
{
    public string text;
    public string next;
}
