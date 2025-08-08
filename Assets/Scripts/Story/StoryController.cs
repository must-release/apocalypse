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

        [Header("Assets")]
        public Transform storyScreen;
        public GameObject[] characters;

        public Coroutine StartStory(string storyInfo, int readBlockCount, int readEntryCount)
        {
            return StartCoroutine(StartStoryCoroutine(storyInfo, readBlockCount, readEntryCount));
        }

        public void FinishStory()
        {
            storyScreen.gameObject.SetActive(false);
        }

        public void PlayNextScript()
        {
            if (0 < _activeStoryPresenters.Count)
            {
                var presentersToComplete = new List<IStoryPresenter>(_activeStoryPresenters);
                presentersToComplete.ForEach(presenter => presenter.CompleteStoryEntry());
                return;
            }

            if (CharacterCGCPresenter.Instance.PlayingStandingEntry != null)
                {
                    if (CharacterCGCPresenter.Instance.PlayingStandingEntry.IsBlockingAnimation)
                        return;

                    // if Standing Animation is unBlockable, Skip Animation.
                    CharacterCGCPresenter.Instance.CompleteStandingAnimation();
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

            if (_storyPresenters.TryGetValue(entry.Type, out IStoryPresenter presenter))
            {
                presenter.ProgressStoryEntry(entry);
                _activeStoryPresenters.Add(presenter);
            }
            else if (entry is StoryCharacterCG standing)
            {
                CharacterCGCPresenter.Instance.HandleCharacterCG(standing);
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

        [SerializeField] private Transform _storyPresentersTransform;

        private StoryUIView _storyUIView;
        private Dictionary<StoryEntry.EntryType, IStoryPresenter> _storyPresenters = new();
        private List<IStoryPresenter> _activeStoryPresenters = new();

        public void Awake()
        {
            Debug.Assert(null != _storyPresentersTransform, "Story Presenters are not assigned in the editor.");
            Debug.Assert(null != storyScreen, "StoryScreen is not assigned in the editor.");
            Debug.Assert(null != characters, "Character Object is not assigned in the editor.");

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

            InitializeStoryPresenters();

            foreach (var character in characters)
            {
                var view = character.GetComponent<CharacterCGView>();
                if (view != null)
                {
                    CharacterCGCPresenter.Instance.RegisterCharacter(view);
                }

                // blinding
                character.SetActive(false);
            }
        }

        private void InitializeStoryPresenters()
        {
            var storyPresenters = _storyPresentersTransform.GetComponents<IStoryPresenter>();
            Debug.Assert(storyPresenters.Length > 0, "No Story Presenters found in StoryController.");

            foreach (var presenter in storyPresenters)
            {
                presenter.Initialize(this, _storyUIView);
                presenter.OnStoryEntryComplete += DeactivateStoryPresenter;
                _storyPresenters.Add(presenter.PresentingEntryType, presenter);
            }
        }

        private void DeactivateStoryPresenter(IStoryPresenter presenter)
        {
            Debug.Assert(null != presenter, "Presenter cannot be null");
            Debug.Assert(_activeStoryPresenters.Contains(presenter), $"Presenter {presenter} is not active");

            _activeStoryPresenters.Remove(presenter);
        }

        private IEnumerator StartStoryCoroutine(string storyInfo, int readBlockCount, int readEntryCount)
        {
            // Set Story Playing true
            IsStoryPlaying = true;

            // Activate Story Screen
            storyScreen.gameObject.SetActive(true);

            // Get dialogue player
            //_dialoguePlayer = UtilityManager.Instance.GetUtilityTool<DialoguePlayer>();

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
