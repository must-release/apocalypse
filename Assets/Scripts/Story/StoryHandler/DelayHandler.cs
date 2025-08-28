using System;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace AD.Story
{
    public class DelayHandler : MonoBehaviour, IStoryEntryHandler
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.Delay;
        public StoryEntry CurrentEntry => _currentDelay;
        public event Action<IStoryEntryHandler> OnStoryEntryComplete;

        public void Initialize(StoryHandleContext context) { }

        public async UniTask ProgressStoryEntry(StoryEntry storyEntry)
        {
            Debug.Assert(storyEntry is StoryDelay, $"{storyEntry} is not a StoryDelay");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in DelayHandler.");

            _currentDelay = storyEntry as StoryDelay;
            _cancellationTokenSource = new CancellationTokenSource();

            OpResult result = await DelayAsync(_currentDelay.Duration, _cancellationTokenSource.Token);
            if (result == OpResult.Success)
            {
                CompleteDelayEntry();
            }
        }

        public void InstantlyCompleteStoryEntry()
        {
            Debug.Assert(null != _currentDelay, "Current Delay is null");

            _cancellationTokenSource?.Cancel();
            CompleteDelayEntry();
        }

        public void ResetHandler()
        {
            _currentDelay = null;
        }


        /****** Private Members ******/

        private StoryDelay _currentDelay;
        private CancellationTokenSource _cancellationTokenSource;

        private void CompleteDelayEntry()
        {
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in DelayHandler.");

            OnStoryEntryComplete.Invoke(this);
        }

        private async UniTask<OpResult> DelayAsync(float duration, CancellationToken cancellationToken = default)
        {
            await UniTask.Delay((int)(duration * 1000), cancellationToken: cancellationToken).SuppressCancellationThrow();

            return cancellationToken.IsCancellationRequested ? OpResult.Aborted : OpResult.Success;
        }
    }
}
