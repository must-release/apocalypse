using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace AD.Story
{
    public class PlayModeHandler : MonoBehaviour, IStoryEntryHandler
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.PlayMode;
        public StoryEntry CurrentEntry => _currentPlayMode;

        public event Action<IStoryEntryHandler> OnStoryEntryComplete;

        public void Initialize(StoryHandleContext context)
        {
            _storyContext = context;
        }

        public UniTask ProgressStoryEntry(StoryEntry entry)
        {
            Debug.Assert(entry is StoryPlayMode, "Entry must be of type StoryPlayMode");

            _currentPlayMode = entry as StoryPlayMode;
            _storyContext.SetCurrentPlayMode(_currentPlayMode.PlayMode);

            BaseUI baseUI = GetPlayModeUI(_currentPlayMode.PlayMode);
            Debug.Assert(baseUI != BaseUI.BaseUICount, "Invalid PlayMode type: " + _currentPlayMode.PlayMode);

            var uiChangeEvt = GameEventFactory.CreateUIChangeEvent(baseUI);
            uiChangeEvt.OnTerminate += CompletePlayModeEntry;
            GameEventManager.Instance.Submit(uiChangeEvt);

            return UniTask.CompletedTask;
        }

        public void InstantlyCompleteStoryEntry()
        {
            // VFX actions are non-blocking and ProgressStoryEntry completes immediately.
            // So, this method might not be called or might not need specific logic.
            // For now, it will be empty.
        }

        public void ResetHandler()
        {
            _currentPlayMode = null;
        }


        /****** Private Members ******/

        private StoryHandleContext _storyContext;
        private StoryPlayMode _currentPlayMode;

        private BaseUI GetPlayModeUI(StoryPlayMode.PlayModeType playMode)
        {
            Debug.Assert((int)StoryPlayMode.PlayModeType.PlayModeTypeCount == 3, "When adding PlayModeType enum, update this method accordingly");

            switch (playMode)
            {
                case StoryPlayMode.PlayModeType.VisualNovel:
                    return BaseUI.Story;
                case StoryPlayMode.PlayModeType.SideDialogue:
                    return BaseUI.Control;
                case StoryPlayMode.PlayModeType.InGameCutScene:
                    return BaseUI.Cutscene;
                default:
                    Logger.Write(LogCategory.Story, "Unsupported PlayMode type: " + playMode, LogLevel.Error);
                    return BaseUI.BaseUICount;
            }
        }

        private void CompletePlayModeEntry()
        {
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete is not assigned in PlayModeHandler.");

            OnStoryEntryComplete.Invoke(this);
        }
    }
}