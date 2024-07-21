using UnityEngine;
using System.Collections;
using UIEnums;
using EventEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewCutscene", menuName = "Event/CutsceneEvent", order = 0)]
public class CutsceneEvent : GameEvent
{
    // Set event Type on load
    public void OnEnable()
    {
        EventType = EVENT_TYPE.CUTSCENE;
    }

    public override bool CheckCompatibility(GameEvent parentEvent, (BASEUI, SUBUI) currentUI)
    {
        // Can be played when there is no event playing
        if (parentEvent == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Play cutscene event
    public override void PlayEvent()
    {
        // Use GameEventManger to start coroutine
        GameEventManager.Instance.StartCoroutineForGameEvents(PlayEventCoroutine());
    }
    IEnumerator PlayEventCoroutine()
    {
        // Change to cutscene UI
        UIController.Instance.ChangeBaseUI(BASEUI.CUTSCENE);

        // Play cutscene
        GamePlayController.Instance.PlayCutscene();

        // Wait for cutscene to end
        while (GamePlayController.Instance.IsCutscenePlaying)
        {
            yield return null;
        }

        // Terminate cutscene event
        GameEventManager.Instance.TerminateGameEvent(this);
    }

}
