using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AD.Story
{
    public class StoryController : MonoBehaviour
    {
        /****** Public Members ******/

        public static StoryController Instance;

        [Header("Parameters")]
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
            if (0 < _activeStoryHandlers.Count)
            {
                var handlersToComplete = new List<IStoryEntryHandler>(_activeStoryHandlers);
                handlersToComplete.ForEach(handler => handler.CompleteStoryEntry());
                return;
            }

            // Check if there is available entry
            StoryEntry entry = StoryModel.Instance.GetNextEntry();
            if (entry == null)
            {
                // Story is over
                IsStoryPlaying = false;
            }
            else
            {
                // Show Story Entry
                ShowStoryEntry(entry);
            }
        }

        public void ShowStoryEntry(StoryEntry entry)
        {
            Debug.Assert(null != entry, "Story Entry cannot be null");

            if (_storyHandlers.TryGetValue(entry.Type, out IStoryEntryHandler handler))
            {
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
            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;

            _storyUIView = UIController.Instance.GetUIView(BaseUI.Story) as StoryUIView;
            Debug.Assert(null != _storyUIView, "StoryUIView is not assigned in StoryController.");

            // Initialize StoryHandleContext here
            _storyContext = new StoryHandleContext(this, _storyUIView);

            InitializeStoryHandlers(); // Renamed method
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
            // Set Story Playing true
            IsStoryPlaying = true;

            // Load Story Text according to the Info
            yield return StoryModel.Instance.LoadStoryText(storyInfo, readBlockCount, readEntryCount);

            // Show first story script When new story is loaded
            StoryEntry entry = StoryModel.Instance.GetFirstEntry();
            if (entry == null)
            {
                Debug.Log("Story initial load error");
                yield break;
            }
            ShowStoryEntry(entry);
        }
    }
}