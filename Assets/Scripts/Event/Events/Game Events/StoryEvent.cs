using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using AD.Story;


public class StoryEvent : GameEventBase<StoryEventInfo>
{
    /****** Public Members ******/

    public override bool            ShouldBeSaved   => true;
    public override GameEventType   EventType       => GameEventType.Story;

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Debug.Assert(null != activeEventTypeCounts, "activeEventTypeCounts is null.");
        
        return true;
    }

    public override void PlayEvent()
    {
        Debug.Assert(null != Info, "Event info is not initialized");

        base.PlayEvent();
        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    public override void TerminateEvent()
    {
        Debug.Assert(null != Info, "Event info is not set before termination");


        // Tell StoryController to finish story
        StoryController.Instance.FinishStory();
        
        if (_eventCoroutine != null)
        {
            StopCoroutine(_eventCoroutine);
            _eventCoroutine = null;
        }

        Info.DestroyInfo();
        Info = null;

        GameEventPool<StoryEvent, StoryEventInfo>.Release(this);
        base.TerminateEvent();
    }

    public override void UpdateStatus()
    {
        base.UpdateStatus();

        StoryController.Instance.GetStoryProgressInfo(out int readBlockCount, out int readEntryCount);
        Info.UpdateStoryProgress(readBlockCount, readEntryCount);
    }


    /****** Private Members ******/

    private Coroutine _eventCoroutine = null;

    private IEnumerator PlayEventCoroutine()
    {
        Debug.Assert( null != Info, "Event info should be set" );

        // Change to story UI
        UIController.Instance.ChangeBaseUI(BaseUI.Story);

        // Start Story
        InputEventProducer.Instance.LockInput(true);
        string story = "StoryScript/" + Info.StoryStage.ToString() + '_' + Info.StoryNumber;
        yield return StoryController.Instance.StartStory(story, Info.ReadBlockCount, Info.ReadEntryCount);
        InputEventProducer.Instance.LockInput(false);

        // Wait for story to end
        yield return new WaitWhile( () => StoryController.Instance.IsStoryPlaying );

        // Terminate story event and play next event
        TerminateEvent();
    }
}