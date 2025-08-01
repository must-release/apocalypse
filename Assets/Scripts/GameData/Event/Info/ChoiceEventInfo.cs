using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;

[Serializable]
[CreateAssetMenu(fileName = "NewChoiceEventInfo", menuName = "EventInfo/ChoiceEvent", order = 0)]
public class ChoiceEventInfo : GameEventInfo, ISerializableEventInfo
{
    /****** Public Members ******/

    public List<string> ChoiceList { get { return _choiceList; } private set { _choiceList = value; }}
    
    public void Initialize(List<string> choices, bool isRuntimeInstance = false)
    {
        Debug.Assert(false == IsInitialized, "Duplicate initialization of GameEventInfo is not allowed.");
        Debug.Assert(null != choices, "Choices cannot be null.");
        Debug.Assert(0 < choices.Count, "Choice list must have at least one item.");

        ChoiceList          = new List<string>(choices);
        IsInitialized       = true;
        IsRuntimeInstance   = true;
    }

    public override GameEventInfo Clone()
    {
        var clone = Instantiate(this);
        clone._choiceList = new List<string>(_choiceList);
        clone.IsRuntimeInstance = true;

        return clone;
    }

    public GameEventDTO ToDTO()
    {
        return new ChoiceEventDTO
        {
            EventType = EventType,
            ChoiceList = new List<string>(_choiceList)
        };
    }
    

    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.Choice;
    }

    protected override void OnValidate()
    {
        if ( null != ChoiceList && 0 < ChoiceList.Count )
            IsInitialized = true;
    }


    /****** Private Members ******/

    [SerializeField] private List<string> _choiceList;
}