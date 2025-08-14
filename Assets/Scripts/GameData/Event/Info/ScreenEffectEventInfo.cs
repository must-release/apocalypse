using UnityEngine;
using UnityEngine.Assertions;
using System;


[Serializable]
[CreateAssetMenu(fileName = "NewScrenEffectEventInfo", menuName = "EventInfo/ScreenEffectEvent", order = 0)]
public class ScreenEffectEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public ScreenEffect ScreenEffectType    => _screenEffectType;
    public float Duration                   => _duration;
    
    public void Initialize(ScreenEffect screenEffectType, float duration, bool isRuntimeInstance = false)
    {
        Debug.Assert( false == IsInitialized,                              "Duplicate initialization of GameEventInfo is not allowed." );
        Debug.Assert( ScreenEffect.ScreenEffectCount != screenEffectType,  "Screen effect is not set properly." );


        _screenEffectType   = screenEffectType;
        _duration           = duration;

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
        EventType = GameEventType.ScreenEffect;
    }

    protected override void OnValidate()
    {
        if ( ScreenEffect.ScreenEffectCount != ScreenEffectType )
            IsInitialized = true;
    }


    /****** Private Members ******/
    [SerializeField] private ScreenEffect _screenEffectType = ScreenEffect.ScreenEffectCount;
    [SerializeField] private float _duration;
}