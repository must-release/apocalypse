using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace AD.Story
{
    public class BGMHandler : MonoBehaviour, IStoryEntryHandler
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.BGM;
        public event Action<IStoryEntryHandler> OnStoryEntryComplete;

        public void Initialize(StoryHandleContext context)
        {
            _context = context;
            Debug.Assert(null != _context.Controller, "StoryController is not assigned in BGMHandler context.");
        }

        public UniTask ProgressStoryEntry(StoryEntry storyEntry)
        {
            Debug.Assert(storyEntry is StoryBGM, $"{storyEntry} is not a StoryBGM");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in BGMHandler.");

            StoryBGM currentBGM = storyEntry as StoryBGM;

            IGameEvent audioEvent;

            switch (currentBGM.Action)
            {
                case StoryBGM.BGMAction.Start:
                    audioEvent = GameEventFactory.CreateBGMEvent(shouldStop: false, currentBGM.BGMName);
                    break;
                case StoryBGM.BGMAction.Stop:
                    audioEvent = GameEventFactory.CreateBGMEvent(shouldStop: true);
                    break;
                default:
                    Logger.Write(LogCategory.GamePlay, $"Unsupported BGMAction: {currentBGM.Action}", LogLevel.Error);
                    OnStoryEntryComplete.Invoke(this);
                    return UniTask.CompletedTask;
            }

            GameEventManager.Instance.Submit(audioEvent);
            
            OnStoryEntryComplete.Invoke(this);

            return UniTask.CompletedTask;
        }

        public void CompleteStoryEntry()
        {
            // BGM actions are non-blocking and ProgressStoryEntry completes immediately.
            // So, this method might not be called or might not need specific logic.
            // For now, it will be empty.
        }


        /****** Private Members ******/

        private StoryHandleContext _context;
    }
}