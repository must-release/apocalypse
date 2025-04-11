using UnityEngine;
using System.Collections.Generic;
using UIEnums;
using EventEnums;
using System.Collections;
using UnityEngine.Assertions;
using System;

[Serializable]
[CreateAssetMenu(fileName = "NewChoiceEvent", menuName = "EventInfo/ChoiceEvent", order = 0)]
public class ChoiceEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public List<string> ChoiceList { get { return _choiceList; } private set { _choiceList = value; }}
    
    public void Initialize(List<string> choices)
    {
        Assert.IsTrue( false == IsInitialized,  "Duplicate initialization of GameEventInfo is not allowed." );
        Assert.IsTrue(null != choices,          "Choices cannot be null.");
        Assert.IsTrue(choices.Count > 0,        "Choice list must have at least one item.");

        ChoiceList      = choices;
        IsInitialized   = true;
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