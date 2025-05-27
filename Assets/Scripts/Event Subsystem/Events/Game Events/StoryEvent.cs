using UnityEngine;
using UIEnums;
using EventEnums;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.AddressableAssets;


public class StoryEvent : GameEventBase<StoryEventInfo>
{
    /****** Public Members ******/

    public override bool            ShouldBeSaved   => true;
    public override GameEventInfo   EventInfo       => _info;
    public override GameEventType   EventType       => GameEventType.Story;

    public override void Initialize(StoryEventInfo eventInfo, IGameEvent parentEvent = null)
    {
        Assert.IsTrue(null != eventInfo && eventInfo.IsInitialized, "Event info is not valid.");

        _info       = eventInfo;
        Status      = EventStatus.Waiting;
        ParentEvent = parentEvent;
    }

    public override bool CheckCompatibility(IReadOnlyDictionary<GameEventType, int> activeEventTypeCounts)
    {
        Assert.IsTrue(null != activeEventTypeCounts, "activeEventTypeCounts is null.");
        
        return true;
    }

    public override void PlayEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not initialized");

        base.PlayEvent();
        _eventCoroutine = StartCoroutine(PlayEventCoroutine());
    }

    // TODO : Find solution for UpdateStoryProgress
    public void UpdateStoryProgress((int, int) storyInfo)
    {
        Assert.IsTrue( null != _info, "Story event is not progressing" );

        // readBlockCount = storyInfo.Item1;
        // readEntryCount = storyInfo.Item2;
    }

    // Terminate story event
    public override void TerminateEvent()
    {
        Assert.IsTrue(null != _info, "Event info is not set before termination");


        // Tell StoryController to finish story
        StoryController.Instance.FinishStory();
        
        if (_eventCoroutine != null)
        {
            StopCoroutine(_eventCoroutine);
            _eventCoroutine = null;
        }

        _info.DestroyInfo();
        _info = null;

        GameEventPool<StoryEvent, StoryEventInfo>.Release(this);
        base.TerminateEvent();
    }


    /****** Private Members ******/

    private Coroutine       _eventCoroutine = null;
    private StoryEventInfo  _info = null;

    private IEnumerator PlayEventCoroutine()
    {
        Assert.IsTrue( null != _info, "Event info should be set" );

        // Change to story UI
        UIController.Instance.ChangeBaseUI(BaseUI.Story);

        // Start Story
        InputEventProducer.Instance.LockInput(true);
        string story = "StoryScripts/" + _info.StoryStage.ToString() + '_' + _info.StoryNumber;
        yield return StoryController.Instance.StartStory(story, _info.ReadBlockCount, _info.ReadEntryCount);
        InputEventProducer.Instance.LockInput(false);

        // Wait for story to end
        yield return new WaitWhile( () => StoryController.Instance.IsStoryPlaying );

        // Terminate story event and play next event
        TerminateEvent();
    }
}