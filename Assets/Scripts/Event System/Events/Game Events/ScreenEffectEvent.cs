using ScreenEffectEnums;
using EventEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewScreenEffect", menuName = "Event/ScreenEffectEvent", order = 0)]
public class ScreenEffectEvent : GameEvent
{
    public SCREEN_EFFECT screenEffect;
    Coroutine effectCoroutine;

    private void OnEnable()
    {
        EventType = EVENT_TYPE.SCREEN_EFFECT;
    }

    // Check compatibility with current event and UI
    public override bool CheckCompatibility(GameEvent parentEvent, BASEUI baseUI, SUBUI subUI)
    {
        return true;
    }

    // Play screent effect event
    public override void PlayEvent()
    {
        // Use GameEventManger to start coroutine
        GameEventManager.Instance.StartCoroutine(PlayEventCoroutine());
    }
    public override IEnumerator PlayEventCoroutine()
    {
        ScreenEffecter screenEffecter = UtilityManager.Instance.GetUtilityTool<ScreenEffecter>();

        switch (screenEffect)
        {
            case SCREEN_EFFECT.FADE_IN:
                effectCoroutine = screenEffecter.FadeIn();
                break;
            case SCREEN_EFFECT.FADE_OUT:
                effectCoroutine = screenEffecter.FadeOut(); 
                break;
        }

        yield return effectCoroutine;
        effectCoroutine = null;

        GameEventManager.Instance.TerminateGameEvent(this);
    }

    // Terminate screen effect event
    public override void TerminateEvent()
    {
        if (effectCoroutine != null)
        {
            Debug.Log("Terminate error: screen effect is not done yet!");
        }
    }
}
