using UnityEngine;
using System;
using AD.Story;

[Serializable]
[CreateAssetMenu(fileName = "NewSideDialogueEventInfo", menuName = "EventInfo/SideDialogueEvent", order = 0)]
public class SideDialogueEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public string   CharacterName   => _characterName;
    public string   Text            => _text;
    public float    TextInterval    => _textInterval;

    public void Initialize(string characterName, string text, float textInterval)
    {
        Debug.Assert(false == string.IsNullOrEmpty(characterName), "Character name cannot be empty in side dialogue event");
        Debug.Assert(false == string.IsNullOrEmpty(text), "Text cannot be empty in side dialogue event.");
        Debug.Assert(0 < textInterval, "Text interval cannot be negative in side dialogue event");

        _text           = text;
        _textInterval   = textInterval;
        _characterName  = characterName;

        IsInitialized       = true;
        IsRuntimeInstance   = true;
    }

    public override GameEventInfo Clone()
    {
        var clone = Instantiate(this);
        clone.IsRuntimeInstance = true;

        return clone;
    }


    /****** Protected Members ******/


    protected override void OnEnable()
    {
        EventType = GameEventType.SideDialogue;
    }

    protected override void OnValidate()
    {
        if (string.IsNullOrEmpty(_characterName))
        {
            Logger.Write(LogCategory.Event, "Character name cannot be empty in side dialogue event");
            return;
        }

        if (string.IsNullOrEmpty(_text))
        {
            Logger.Write(LogCategory.Event, "Text cannot be empty in side dialogue event");
            return;
        }

        IsInitialized = true;
    }


    /****** Private Members ******/
    [SerializeField] private string _characterName;
    [SerializeField] private string _text;
    [SerializeField] private float  _textInterval;
}