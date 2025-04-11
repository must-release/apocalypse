using ScreenEffectEnums;
using EventEnums;
using UnityEngine;
using UnityEngine.Assertions;
using System;


[Serializable]
[CreateAssetMenu(fileName = "NewScrenEffectEvent", menuName = "EventInfo/ScreenEffectEvent", order = 0)]
public class ScreenEffectEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public ScreenEffect ScreenEffectType { get { return _screenEffectType; } private set { _screenEffectType = value; }}
    
    public void Initialize(ScreenEffect screenEffectType)
    {
        Assert.IsTrue( false == IsInitialized,                              "Duplicate initialization of GameEventInfo is not allowed." );
        Assert.IsTrue( ScreenEffect.ScreenEffectCount != screenEffectType,  "Screen effect is not set properly." );


        ScreenEffectType    = screenEffectType;
        IsInitialized       = true;
    }


    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.ScreenEffect;
    }

    protected override void OnValidate()
    {
        if ( ScreenEffect.ScreenEffectCount != ScreenEffectType )
            IsInitialized = true;
    }


    /****** Private Members ******/
    [SerializeField] private ScreenEffect _screenEffectType = ScreenEffect.ScreenEffectCount;
}