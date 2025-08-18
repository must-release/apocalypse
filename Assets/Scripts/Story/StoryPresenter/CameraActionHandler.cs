using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using AD.Camera;

namespace AD.Story
{
    public class CameraActionHandler : MonoBehaviour, IStoryEntryHandler
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.CameraAction;
        public StoryEntry CurrentEntry => _currentCameraAction;
        public event Action<IStoryEntryHandler> OnStoryEntryComplete;

        public void Initialize(StoryHandleContext context) { }

        public UniTask ProgressStoryEntry(StoryEntry storyEntry)
        {
            Debug.Assert(storyEntry is StoryCameraAction, $"{storyEntry} is not a StoryCameraAction");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in CameraActionHandler.");

            _currentCameraAction = storyEntry as StoryCameraAction;

            var actionType = _currentCameraAction.ActionType;
            var cameraName = _currentCameraAction.CameraName;
            var targetName = _currentCameraAction.TargetName;
            var isTargetPlayer = _currentCameraAction.IsTargetPlayer;
            var cameraEvent = GameEventFactory.CreateCameraEvent(actionType, cameraName, isTargetPlayer, targetName);
            cameraEvent.OnTerminate += () =>
            {
                _currentCameraAction = null;
                CompleteStoryEntry();
            };

            GameEventManager.Instance.Submit(cameraEvent);

            return UniTask.CompletedTask;
        }

        public void CompleteStoryEntry()
        {
            Debug.Assert(null != _currentCameraAction, "Current camera action is null");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in CameraActionHandler.");

            if (null != _currentCameraAction)
                return;

            OnStoryEntryComplete.Invoke(this);
        }

        public void ResetHandler()
        {
            _context = null;
            _currentCameraAction = null;
        }

        /****** Private Members ******/

        private StoryCameraAction _currentCameraAction;
    }
}