using UnityEngine;
using System.Collections;
using UIEnums;
using EventEnums;

[System.Serializable]
[CreateAssetMenu(fileName = "NewCutscene", menuName = "Event/CutsceneEvent", order = 0)]
public class CutsceneEvent : GameEvent
{
    private Coroutine cutsceneCoroutine;

    // Set event Type on load
    public void OnEnable()
    {
        EventType = EventEnums.GameEventType.Cutscene;
    }

    // Check compatibility with current event and UI
    public override bool CheckCompatibility(GameEvent parentEvent, BaseUI baseUI, SubUI subUI)
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
        cutsceneCoroutine = GameEventManager.Instance.StartCoroutine(PlayEventCoroutine());
    }
    public override IEnumerator PlayEventCoroutine()
    {
        // Change to cutscene UI
        UIController.Instance.ChangeBaseUI(BaseUI.Cutscene);

        // Play cutscene
        GamePlayManager.Instance.PlayCutscene();

        // Wait for cutscene to end
        while (GamePlayManager.Instance.IsCutscenePlaying)
        {
            yield return null;
        }

        // Terminate cutscene event
        GameEventManager.Instance.TerminateGameEvent(this);
    }

    // Terminate cutscene event
    public override void TerminateEvent()
    {
        GameEventManager.Instance.StopCoroutine(cutsceneCoroutine);
    }
}
