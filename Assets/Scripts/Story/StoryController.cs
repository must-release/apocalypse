using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AD.Story
{
    public class StoryController : MonoBehaviour
    {
        /****** Public Members ******/

        public static StoryController Instance { get; private set; }

        public bool IsStoryPlaying { get; private set; }


        public Coroutine StartStory(string storyInfo, int readBlockCount, int readEntryCount)
        {
            return StartCoroutine(StartStoryCoroutine(storyInfo, readBlockCount, readEntryCount));
        }

        public void FinishStory()
        {

        }

        public void PlayNextScript()
        {
            StoryEntry nextEntry = StoryModel.Instance.PeekFirstEntry();

            if (null == nextEntry)
            {
                IsStoryPlaying = false;
                return;
            }

            var handlerToComplete = _activeStoryHandlers.FirstOrDefault(h => h.CurrentEntry.IsSingleExecuted || h.PresentingEntryType == nextEntry.Type);
            if (null != handlerToComplete)
            {
                handlerToComplete.InstantlyCompleteStoryEntry();
                return;
            }

            if (nextEntry.IsSingleExecuted && 0 < _activeStoryHandlers.Count)
            {
                var handlersToComplete = new List<IStoryEntryHandler>(_activeStoryHandlers);
                handlersToComplete.ForEach(handler => handler.InstantlyCompleteStoryEntry());
                return;
            }

            // If all checks pass, consume and show the entry.
            StoryEntry entry = StoryModel.Instance.GetNextEntry();
            ShowStoryEntry(entry);
        }

        public void ShowStoryEntry(StoryEntry entry)
        {
            Debug.Assert(null != entry, "Story Entry cannot be null");

            if (_storyHandlers.TryGetValue(entry.Type, out IStoryEntryHandler handler))
            {
                Logger.Write(LogCategory.Story, $"Progressing {entry.Type} entry");

                _activeStoryHandlers.Add(handler);
                handler.ProgressStoryEntry(entry);
            }
            else
            {
                Debug.Log($"story entry error: no such entry {entry}");
            }
        }


        public void ProcessSelectedChoice(StoryChoiceOption choiceOption)
        {
            StoryModel.Instance.CurrentStoryBranch = choiceOption.BranchName;
        }

        public void GetStoryProgressInfo(out int readBlockCount, out int readEntryCount)
        {
            readBlockCount = StoryModel.Instance.ReadBlockCount;
            readEntryCount = StoryModel.Instance.ReadEntryCount;
        }


        /****** Private Menbers ******/

        [SerializeField] private Transform _storyHandlersTransform;

        private StoryUIView _storyUIView;
        private StoryHandleContext _storyContext;
        private Dictionary<StoryEntry.EntryType, IStoryEntryHandler> _storyHandlers = new();
        private List<IStoryEntryHandler> _activeStoryHandlers = new();
        // Removed _bufferedEntry

        public void Awake()
        {
            Debug.Assert(null != _storyHandlersTransform, "Story Handlers are not assigned in the editor.");

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _storyUIView = UIController.Instance.GetUIView(BaseUI.Story) as StoryUIView;
            Debug.Assert(null != _storyUIView, "StoryUIView is not assigned in StoryController.");

            // Initialize StoryHandleContext here
            _storyContext = new StoryHandleContext(this, _storyUIView);

            InitializeStoryHandlers();
        }

        private void InitializeStoryHandlers()
        {
            var storyHandlers = _storyHandlersTransform.GetComponents<IStoryEntryHandler>();
            Debug.Assert(storyHandlers.Length > 0, "No Story Handlers found in StoryController.");

            foreach (var handler in storyHandlers)
            {
                handler.Initialize(_storyContext);
                handler.OnStoryEntryComplete += DeactivateStoryHandler;
                _storyHandlers.Add(handler.PresentingEntryType, handler);
            }
        }

        private void DeactivateStoryHandler(IStoryEntryHandler handler)
        {
            Debug.Assert(null != handler, "Handler cannot be null");
            Debug.Assert(_activeStoryHandlers.Contains(handler), $"Handler {handler} is not active");

            bool isAutoProgress = handler.CurrentEntry.IsAutoProgress;

            handler.ResetHandler();
            _activeStoryHandlers.Remove(handler); // Release Handler

            if (true == isAutoProgress)
            {
                PlayNextScript();
            }
        }

        private IEnumerator StartStoryCoroutine(string storyInfo, int readBlockCount, int readEntryCount)
        {
            IsStoryPlaying = true;

            yield return StoryModel.Instance.LoadStoryText(storyInfo, readBlockCount, readEntryCount);

            StoryEntry entry = StoryModel.Instance.PeekFirstEntry();
            Debug.Assert(null != entry, "Story Entry cannot be null");

            if (entry is StoryPlayMode)
            {
                StoryModel.Instance.GetNextEntry();
                ShowStoryEntry(entry);
                yield break;
            }
            
            StoryPlayMode storyPlayMode = new StoryPlayMode(StoryPlayMode.PlayModeType.VisualNovel);
            ShowStoryEntry(storyPlayMode);
        }
    }
}