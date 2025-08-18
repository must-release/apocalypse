using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace AD.Story
{
    public class PlayModeHandler : MonoBehaviour, IStoryEntryHandler
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.PlayMode;
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
            GameEventManager.Instance.Submit(uiChangeEvt);

            _currentPlayMode = null;
            CompleteStoryEntry();

            return UniTask.CompletedTask;
        }

        public void CompleteStoryEntry()
        {
            if (null != _currentPlayMode)
                return;

            OnStoryEntryComplete.Invoke(this);

            _storyContext.Controller.PlayNextScript();
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
    }
}