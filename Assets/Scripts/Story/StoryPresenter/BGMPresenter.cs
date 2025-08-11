using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using AD.Audio;

namespace AD.Story
{
    public class BGMPresenter : MonoBehaviour, IStoryEntryHandler // Changed to IStoryEntryHandler
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.BGM;
        public event Action<IStoryEntryHandler> OnStoryEntryComplete; // Changed to IStoryEntryHandler

        private StoryContext _context; // Store the context

        public void Initialize(StoryContext context) // Changed signature
        {
            _context = context;
            Debug.Assert(null != _context.Controller, "StoryController is not assigned in BGMPresenter context.");
        }

        public async UniTask ProgressStoryEntry(StoryEntry storyEntry)
        {
            Debug.Assert(storyEntry is StoryBGM, $"{storyEntry} is not a StoryBGM");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in BGMPresenter.");

            StoryBGM currentBGM = storyEntry as StoryBGM;

            AudioAction audioAction;
            switch (currentBGM.Action)
            {
                case StoryBGM.BGMAction.Start:
                    audioAction = AudioAction.Play;
                    break;
                case StoryBGM.BGMAction.Stop:
                    audioAction = AudioAction.Stop;
                    break;
                default:
                    Debug.LogError($"Unsupported BGMAction: {currentBGM.Action}");
                    OnStoryEntryComplete.Invoke(this);
                    return;
            }

            IGameEvent audioEvent = GameEventFactory.CreateAudioEvent(
                isBgm: true,
                action: audioAction,
                clipName: currentBGM.BGMName,
                volume: 1.0f
            );

            audioEvent.PlayEvent();
            OnStoryEntryComplete.Invoke(this);
        }

        public void CompleteStoryEntry()
        {
            // BGM actions are non-blocking and ProgressStoryEntry completes immediately.
            // So, this method might not be called or might not need specific logic.
            // For now, it will be empty.
        }


        /****** Private Members ******/

        // private StoryController _storyController; // Now accessed via _context.Controller
    }
}