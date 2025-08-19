using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace AD.Story
{
    public class VFXHandler : MonoBehaviour, IStoryEntryHandler
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.VFX;
        public StoryEntry CurrentEntry => _currentVFX;

        public event Action<IStoryEntryHandler> OnStoryEntryComplete;

        public void Initialize(StoryHandleContext context)
        {
            _context = context;
            Debug.Assert(null != _context.Controller, "StoryController is not assigned in VFXHandler context.");
        }

        public async UniTask ProgressStoryEntry(StoryEntry storyEntry)
        {
            Debug.Assert(storyEntry is StoryVFX, $"{storyEntry} is not a StoryVFX");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in VFXHandler.");

            _currentVFX = storyEntry as StoryVFX;

            ScreenEffect screenEffectType = ScreenEffect.ScreenEffectCount;
            switch (_currentVFX.VFX)
            {
                case StoryVFX.VFXType.ScreenFadeIn:
                    screenEffectType = ScreenEffect.FadeIn;
                    break;
                case StoryVFX.VFXType.ScreenFadeOut:
                    screenEffectType = ScreenEffect.FadeOut;
                    break;
                default:
                    Debug.Assert(false, "Undefined value assigned in VFXType.");
                    break;
            }

            IGameEvent screenEffectEvent = GameEventFactory.CreateScreenEffectEvent(screenEffectType, _currentVFX.Duration);

            GameEventManager.Instance.Submit(screenEffectEvent);

            OnStoryEntryComplete.Invoke(this);

            await UniTask.CompletedTask;
        }

        public void InstantlyCompleteStoryEntry()
        {
            // VFX actions are non-blocking and ProgressStoryEntry completes immediately.
            // So, this method might not be called or might not need specific logic.
            // For now, it will be empty.
        }

        public void ResetHandler()
        {
            _currentVFX = null;
        }


        /****** Private Members ******/

        private StoryHandleContext _context;
        private StoryVFX _currentVFX;
    }
}