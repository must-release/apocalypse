using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace AD.Story
{
    public class CharacterCGPresenter : MonoBehaviour, IStoryEntryHandler // Changed to IStoryEntryHandler
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.CharacterCG;
        public event Action<IStoryEntryHandler> OnStoryEntryComplete; // Changed to IStoryEntryHandler

        public void Initialize(StoryHandleContext context)
        {
            _context = context;
            Debug.Assert(null != _context.Controller, "StoryController is not assigned in CharacterCGPresenter context.");
            
            // Access UI elements directly from context
            Debug.Assert(null != _context.UIView, "StoryUIView is not assigned in CharacterCGPresenter context.");
            // _characterHolder = _context.UIView.CharacterHolder;
            // Debug.Assert(null != _characterHolder, "CharacterHolder is not assigned in CharacterCGPresenter.");
        }

        public async UniTask ProgressStoryEntry(StoryEntry storyEntry)
        {
            Debug.Assert(storyEntry is StoryCharacterCG, $"{storyEntry} is not a StoryCharacterCG");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in CharacterCGPresenter.");

            _currentCharacterCG = storyEntry as StoryCharacterCG;
            _isCompleted = false; // Re-added flag

            Sprite expressionSprite                         = _model.GetExpressionSprite(_currentCharacterCG.Name, _currentCharacterCG.Expression);
            string id                                       = _currentCharacterCG.Name;
            float duration                                  = _defaultCGAnimationDuration / _currentCharacterCG.AnimationSpeed;
            Vector2 targetPosition                          = GetTargetPosition(_currentCharacterCG.TargetPosition);
            StoryCharacterCG.AnimationType animationType    = _currentCharacterCG.Animation;

            _characterHolder.SetSprite(id, expressionSprite);
            _activeCGAnimationTween = _characterHolder.DisplayCG(id, duration, targetPosition, animationType);

            await _activeCGAnimationTween; // Reverted to direct await

            if (_isCompleted) return; // Flag logic

            OnStoryEntryComplete.Invoke(this);
        }

        public void CompleteStoryEntry()
        {
            Debug.Assert(null != _currentCharacterCG, "Current CharacterCG is null");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in CharacterCGPresenter.");

            // if Animation is Blocker, CANNOT complete this Entry.
            if (true == _currentCharacterCG.IsBlockingAnimation) return;

            _isCompleted = true;

            _activeCGAnimationTween?.Complete(); // Reverted to Complete()
            _activeCGAnimationTween = null;
            OnStoryEntryComplete.Invoke(this);
            _currentCharacterCG = null;
        }


        /****** Private Members ******/

        // private StoryController         _storyController; // Now accessed via _context.Controller
        private StoryHandleContext _context; // Store the context
        private StoryCharacterCG _currentCharacterCG;

        private CharacterCGModel _model;
        [SerializeField] private CharacterHolder  _characterHolder;
        
        private Tween _activeCGAnimationTween;
        private bool _isCompleted; // Re-added field
        private float _defaultCGAnimationDuration = 3f;
        private Vector2 _leftPosition = new Vector2(-500, -219);
        private Vector2 _rightPosition = new Vector2(500, -219);
        private Vector2 _centerPosition = new Vector2(0, -219);


        private async void Start()
        {
            _model = new CharacterCGModel();
            await _model.AsyncLoadCharacterExpressionAsset();
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