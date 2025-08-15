using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace AD.Story
{
    public class CameraActionHandler : MonoBehaviour, IStoryEntryHandler
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.CameraAction;
        public StoryEntry CurrentEntry => _currentCameraAction;
        public event Action<IStoryEntryHandler> OnStoryEntryComplete;

        public void Initialize(StoryHandleContext context)
        {
            Debug.Assert(null != context, "StoryHandleContext cannot be null in CameraActionHandler.");
            Debug.Assert(context.IsValid, "StoryHandleContext is not valid in CameraActionHandler.");

            _context = context;
        }

        public async UniTask ProgressStoryEntry(StoryEntry storyEntry)
        {
            Debug.Assert(storyEntry is StoryCameraAction, $"{storyEntry} is not a StoryCameraAction");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in CameraActionHandler.");

            _currentCameraAction = storyEntry as StoryCameraAction;

            // Implement camera action logic here
            await ExecuteCameraAction(_currentCameraAction);

            OnStoryEntryComplete.Invoke(this);
        }

        public void CompleteStoryEntry()
        {
            Debug.Assert(null != _currentCameraAction, "Current camera action is null");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in CameraActionHandler.");

            // Logic to finalize the camera action if needed
            OnStoryEntryComplete.Invoke(this);
            _currentCameraAction = null;
        }

        /****** Private Members ******/

        private StoryHandleContext _context;
        private StoryCameraAction _currentCameraAction;

        private async UniTask ExecuteCameraAction(StoryCameraAction cameraAction)
        {
            // Placeholder for actual camera action execution logic
            await UniTask.Delay(1000); // Simulate delay for camera action
        }
    }
}