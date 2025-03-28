using ScreenEffectEnums;
using EventEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIEnums;
using UnityEngine.Assertions;

/*
 * Screen effect event. Must be removed.
 */

public class ScreenEffectEvent : GameEvent
{
    /****** Public Members ******/

    public void SetEventInfo(ScreenEffectEventInfo eventInfo)
    {
        Assert.IsTrue(eventInfo.IsInitialized, "Event info is not initialized");

        _effectEventInfo = eventInfo;
    }

    public override bool CheckCompatibility(GameEvent parentEvent, BaseUI baseUI, SubUI subUI)
    {
        return true;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue(null != _effectEventInfo, "Event info is not initialized");

        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        Assert.IsTrue(null != _effectEventInfo, "Event info is not set before termination");

        if (_eventCoroutine != null)
        {
            StopCoroutine(_eventCoroutine);
            _eventCoroutine = null;
        }

        ScriptableObject.Destroy(_effectEventInfo);
        _effectEventInfo = null;

        GameEventPool<ScreenEffectEvent>.Release(this);
    }

    public override GameEventInfo GetEventInfo()
    {
        return _effectEventInfo;
    }


    /****** Private Members ******/
    private Coroutine               _eventCoroutine     = null;
    private ScreenEffectEventInfo   _effectEventInfo    = null;

    private IEnumerator PlayEventCoroutine()
    {
        Assert.IsTrue( null != _effectEventInfo, "Event info should be set" );


        ScreenEffecter screenEffecter = UtilityManager.Instance.GetUtilityTool<ScreenEffecter>();

        switch (_effectEventInfo.ScreenEffectType)
        {
            case ScreenEffect.FadeIn:
                _eventCoroutine = screenEffecter.FadeIn();
                break;
            case ScreenEffect.FadeOut:
                _eventCoroutine = screenEffecter.FadeOut(); 
                break;
        }

        yield return _eventCoroutine;
        _eventCoroutine = null;

        GameEventManager.Instance.TerminateGameEvent(this);
    }
}


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