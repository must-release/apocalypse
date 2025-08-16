using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace AD.Story
{
    public class CharacterCGHandler : MonoBehaviour, IStoryEntryHandler // Changed to IStoryEntryHandler
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.CharacterCG;
        public StoryEntry CurrentEntry => _currentCharacterCG;
        public event Action<IStoryEntryHandler> OnStoryEntryComplete; // Changed to IStoryEntryHandler

        public void Initialize(StoryHandleContext context)
        {
            Debug.Assert(null != context, "StoryHandleContext cannot be null in CharacterCGHandler.");
            Debug.Assert(context.IsValid, "StoryHandleContext is not valid in CharacterCGHandler.");

            _context = context;
            _characterCGHolder = _context.UIView.CharacterHolder;

            Debug.Assert(null != _characterCGHolder, "CharacterHolder is not assigned in CharacterCGHandler.");
        }

        public async UniTask ProgressStoryEntry(StoryEntry storyEntry)
        {
            Debug.Assert(storyEntry is StoryCharacterCG, $"{storyEntry} is not a StoryCharacterCG");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in CharacterCGHandler.");

            _currentCharacterCG = storyEntry as StoryCharacterCG;
            _isCompleted = false; // Re-added flag

            Sprite expressionSprite                         = _model.GetExpressionSprite(_currentCharacterCG.Name, _currentCharacterCG.Expression);
            string id                                       = _currentCharacterCG.Name;
            float duration                                  = _defaultCGAnimationDuration / _currentCharacterCG.AnimationSpeed;
            Vector2 targetPosition                          = GetTargetPosition(_currentCharacterCG.TargetPosition);
            StoryCharacterCG.AnimationType animationType    = _currentCharacterCG.Animation;

            _characterCGHolder.SetSprite(id, expressionSprite);
            _activeCGAnimationTween = _characterCGHolder.DisplayCG(id, duration, targetPosition, animationType);

            await _activeCGAnimationTween; // Reverted to direct await

            if (_isCompleted)
                return;

            OnStoryEntryComplete.Invoke(this);
        }

        public void CompleteStoryEntry()
        {
            Debug.Assert(null != _currentCharacterCG, "Current CharacterCG is null");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in CharacterCGHandler.");

            // if Animation is Blocker, CANNOT complete this Entry.
            if (true == _currentCharacterCG.IsBlockingAnimation)
                return;

            _isCompleted = true;

            _activeCGAnimationTween?.Complete(); // Reverted to Complete()
            _activeCGAnimationTween = null;
            OnStoryEntryComplete.Invoke(this);
            _currentCharacterCG = null;
        }

        public void ResetHandler()
        {
            _currentCharacterCG = null;
            _activeCGAnimationTween = null;
            _isCompleted = false;
        }


        /****** Private Members ******/

        private StoryHandleContext _context;
        private StoryCharacterCG _currentCharacterCG;
        
        private CharacterCGModel _model;
        private CharacterCGHolder  _characterCGHolder;
        
        private Tween _activeCGAnimationTween;
        private bool _isCompleted; // Re-added field
        private float _defaultCGAnimationDuration = 0.7f;
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