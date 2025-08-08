using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace AD.Story
{
    public class CharacterCGCPresenter : MonoBehaviour, IStoryPresenter
    {
        /****** Public Members ******/
        public static CharacterCGCPresenter Instance { get; private set; }
        public StoryCharacterCG PlayingStandingEntry { get; private set; }
        
        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.CharacterCG;
        public event Action<IStoryPresenter> OnStoryEntryComplete;

        public void Initialize(StoryController storyController, StoryUIView uiView)
        {

        }

        public async UniTask ProgressStoryEntry(StoryEntry storyEntry)
        {
            Debug.Assert(storyEntry is StoryCharacterCG, $"{storyEntry} is not a StoryCharacterCG");
            var cgInfo = storyEntry as StoryCharacterCG;

            HandleCharacterCG(cgInfo);
        }

        public void CompleteStoryEntry()
        {

        }

        public void RegisterCharacter(CharacterCGView view)
        {
            if (false == _characterViewPool.Contains(view))
            {
                _characterViewPool.Add(view);
                view.SetActive(false);
            }
        }

        public void HandleCharacterCG(StoryCharacterCG cgInfo)
        {
            PlayingStandingEntry = cgInfo;

            float duration = _defaultCGAnimationDuration / cgInfo.AnimationSpeed;
            Vector2 targetPosition = GetTargetPosition(cgInfo.TargetPosition);

            CharacterCGView characterView = GetCharacterView(cgInfo.Name);
            Sprite expressionSprite = _model.GetExpressionSprite(cgInfo.Name, cgInfo.Expression);

            Debug.Assert(null != expressionSprite, "expressionSprite is null.");

            characterView.SetSprite(expressionSprite);

            Tween currentTween = null;

            switch (cgInfo.Animation)
            {
                case StoryCharacterCG.AnimationType.Appear:
                    characterView.SetPosition(targetPosition);
                    characterView.SetActive(true);
                    currentTween = characterView.AsyncFade(0f, 1f, duration);
                    break;

                case StoryCharacterCG.AnimationType.Disappear:
                    currentTween = characterView.AsyncFade(1f, 0f, duration);
                    ReleaseCharacterView(cgInfo.Name);
                    break;

                case StoryCharacterCG.AnimationType.Move:
                    currentTween = characterView.AsyncMove(targetPosition, duration);
                    break;
            }

            // If Animation active
            if (null != currentTween)
            {
                _activeCGAnimationTween = currentTween;

                _activeCGAnimationTween.onComplete += () =>
                {
                    PlayingStandingEntry = null;
                    _activeCGAnimationTween = null;
                };
            }
            else
            {
                PlayingStandingEntry = null;
            }
        }

        public void CompleteStandingAnimation()
        {
            _activeCGAnimationTween?.Complete();
            _activeCGAnimationTween = null;
        }


        /****** Private Members ******/

        private CharacterCGModel _model;
        private List<CharacterCGView> _characterViewPool = new List<CharacterCGView>();
        private Dictionary<string, CharacterCGView> _activeCharacterViews = new Dictionary<string, CharacterCGView>();
        private Tween _activeCGAnimationTween;
        private float _defaultCGAnimationDuration = 0.7f;
        private Vector2 _leftPosition = new Vector2(-500, -219);
        private Vector2 _rightPosition = new Vector2(500, -219);
        private Vector2 _centerPosition = new Vector2(0, -219);

        private void Awake()
        {
            if (null == Instance)
            {
                Instance = this;
                _model = new CharacterCGModel();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private async void Start()
        {
            await _model.AsyncLoadCharacterExpressionAsset();
        }

        private void ReleaseCharacterView(string characterID)
        {
            if (_activeCharacterViews.TryGetValue(characterID, out CharacterCGView view))
            {
                view.gameObject.SetActive(false);
                _activeCharacterViews.Remove(characterID);
            }
        }

        private CharacterCGView GetCharacterView(string characterID)
        {
            if (_activeCharacterViews.TryGetValue(characterID, out CharacterCGView view))
            {
                return view;
            }

            // Find an inactive view from the pool
            CharacterCGView inactiveView = _characterViewPool.FirstOrDefault(v => false == v.gameObject.activeInHierarchy);
            Debug.Assert(null != inactiveView, $"No available character GameObject found for '{characterID}'.");

            // Assign the inactive view to the characterID and activate it
            _activeCharacterViews[characterID] = inactiveView;
            return inactiveView;
        }

        private Vector2 GetTargetPosition(StoryCharacterCG.TargetPositionType targetPositionType)
        {
            switch (targetPositionType)
            {
                case StoryCharacterCG.TargetPositionType.Left:
                    return _leftPosition;
                case StoryCharacterCG.TargetPositionType.Right:
                    return _rightPosition;
                case StoryCharacterCG.TargetPositionType.Center:
                default:
                    return _centerPosition;
            }
        }
    }
}