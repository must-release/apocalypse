using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace AD.Story
{
    public class SFXHandler : MonoBehaviour, IStoryEntryHandler
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.SFX;
        public event Action<IStoryEntryHandler> OnStoryEntryComplete;

        public void Initialize(StoryHandleContext context)
        {
            _context = context;
            Debug.Assert(null != _context.Controller, "StoryController is not assigned in SFXHandler context.");
        }

        public async UniTask ProgressStoryEntry(StoryEntry storyEntry)
        {
            Debug.Assert(storyEntry is StorySFX, $"{storyEntry} is not a StorySFX");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in SFXHandler.");

            StorySFX currentSFX = storyEntry as StorySFX;

            IGameEvent audioEvent = GameEventFactory.CreateSFXEvent(currentSFX.SFXName);

            GameEventManager.Instance.Submit(audioEvent);
            
            OnStoryEntryComplete.Invoke(this);

            await UniTask.CompletedTask;
        }

        public void CompleteStoryEntry()
        {
            // SFX actions are non-blocking and ProgressStoryEntry completes immediately.
            // So, this method might not be called or might not need specific logic.
            // For now, it will be empty.
        }


        /****** Private Members ******/

        private StoryHandleContext _context;
    }
}
