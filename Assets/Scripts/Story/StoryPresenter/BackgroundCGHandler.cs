using UnityEngine;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace AD.Story
{
    public class BackgroundCGHandler : MonoBehaviour, IStoryEntryHandler
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.BackgroundCG;
        public event Action<IStoryEntryHandler> OnStoryEntryComplete;

        public void Initialize(StoryHandleContext context)
        {
            Debug.Assert(null != context, "StoryHandleContext cannot be null in BackgroundCGHandler.");
            Debug.Assert(context.IsValid, "StoryHandleContext is not valid in BackgroundCGHandler.");

            _context = context;
            _backgroundImage = _context.UIView.BackgroundImage;

            Debug.Assert(null != _backgroundImage, "BackgroundImage is not assigned in BackgroundCGHandler.");
        }

        public async UniTask ProgressStoryEntry(StoryEntry storyEntry)
        {
            Debug.Assert(storyEntry is StoryBackgroundCG, $"{storyEntry} is not a StoryBackgroundCG");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in BackgroundCGHandler.");

            _currentBackgroundCG = storyEntry as StoryBackgroundCG;
            _isCompleted = false;

            Sprite bgSprite = _model.GetBackgroundSprite(_currentBackgroundCG.Chapter, _currentBackgroundCG.ImageName);
            _backgroundImage.SetSprite(bgSprite);

            _activeCGAnimationTween = _backgroundImage.PlayAnimation(
                _currentBackgroundCG.Animation,
                _currentBackgroundCG.AnimationDuration,
                _currentBackgroundCG.TargetPosition
            );

            await _activeCGAnimationTween;

            if (_isCompleted)
                return;

            OnStoryEntryComplete.Invoke(this);
        }

        public void CompleteStoryEntry()
        {
            Debug.Assert(null != _currentBackgroundCG, "Current BackgroundCG is null");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in BackgroundCGHandler.");

            // if Animation is Blocker, CANNOT complete this Entry.
            // if (true == _currentBackgroundCG.IsBlockingAnimation)
            //     return;

            _isCompleted = true;

            _activeCGAnimationTween?.Complete();
            _activeCGAnimationTween = null;
            OnStoryEntryComplete.Invoke(this);
            _currentBackgroundCG = null;
        }


        /****** Private Members ******/

        private StoryHandleContext _context;
        private StoryBackgroundCG _currentBackgroundCG;
        private BackgroundCGModel _model;
        private BackgroundImage _backgroundImage;
        
        private Tween _activeCGAnimationTween;
        private bool _isCompleted;
        private float _defaultCGAnimationDuration = 0.7f;

        private async void Start()
        {
            _model = new BackgroundCGModel();
            await _model.AsyncLoadBackgroundCGAsset();
        }
    }
}