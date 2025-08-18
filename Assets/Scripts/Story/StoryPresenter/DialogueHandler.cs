using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.Story
{
    public class DialogueHandler : MonoBehaviour, IStoryEntryHandler
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.Dialogue;
        public event Action<IStoryEntryHandler> OnStoryEntryComplete;

        public void Initialize(StoryHandleContext context)
        {
            Debug.Assert(null != context, "StoryHandleContext cannot be null in DialogueHandler.");
            Debug.Assert(context.IsValid, "StoryHandleContext is not valid in DialogueHandler.");

            _context = context;
            _dialogueBox = _context.UIView.DialogueBox;
            
            Debug.Assert(null != _dialogueBox, "DialogueBox is not assigned in DialogueHandler.");
        }

        public async UniTask ProgressStoryEntry(StoryEntry storyEntry)
        {
            Debug.Assert(storyEntry is StoryDialogue, $"{storyEntry} is not a StoryDialogue");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in DialogueHandler.");

            _currentDialogue = storyEntry as StoryDialogue;
            _cancellationTokenSource = new CancellationTokenSource();

            switch (_context.CurrentPlayMode)
            {
                case StoryPlayMode.PlayModeType.VisualNovel:
                    await ProgressVisualNovelDialogue();
                    break;
                case StoryPlayMode.PlayModeType.SideDialogue:
                    ProgressSideDialogue();
                    break;
                case StoryPlayMode.PlayModeType.InGameCutScene:
                    await ProgressCutsceneDialogue();
                    break;
                default:
                    Logger.Write(LogCategory.Story, "Invalid PlayMode type: " + _context.CurrentPlayMode);
                    break;
            }
        }

        // TODO: Change this method name to InstantlyCompleteStoryEntry
        public void CompleteStoryEntry()
        {
            Debug.Assert(null != _currentDialogue, "Current dialogue is null");
            Debug.Assert(StoryPlayMode.PlayModeType.SideDialogue != _context.CurrentPlayMode, "Cannot instantly complete dialogue in SideDialogue mode.");

            _cancellationTokenSource?.Cancel();
            CompleteDialogueEntry();
        }

        /****** Private Members ******/

        private StoryHandleContext      _context;
        private DialogueBox             _dialogueBox;
        private StoryDialogue           _currentDialogue;
        private CancellationTokenSource _cancellationTokenSource;

        private async UniTask ProgressVisualNovelDialogue()
        {
            Debug.Assert(null != _currentDialogue, "Current dialogue is null in DialogueHandler.");
            Debug.Assert(null != _cancellationTokenSource, "CancellationTokenSource is not initialized in DialogueHandler.");

            _dialogueBox.SetName(_currentDialogue.Name);

            OpResult result = await _dialogueBox.DisplayText(_currentDialogue.Text, CalculateTextInterval(_currentDialogue.TextSpeed), _cancellationTokenSource.Token);
            if (result == OpResult.Success)
            {
                CompleteDialogueEntry();
            }
        }

        private void ProgressSideDialogue()
        {
            Debug.Assert(null != _currentDialogue, "Current dialogue is null in DialogueHandler.");
            Debug.Assert(null != _cancellationTokenSource, "CancellationTokenSource is not initialized in DialogueHandler.");
            Debug.Assert(StoryPlayMode.PlayModeType.SideDialogue == _context.CurrentPlayMode, $"Current play mode {_context.CurrentPlayMode} is not Side Dialogue.");

            var sideDialogueEvent = GameEventFactory.CreateSideDialogueEvent(_currentDialogue.Name, _currentDialogue.Text, CalculateTextInterval(_currentDialogue.TextSpeed));
            GameEventManager.Instance.Submit(sideDialogueEvent);

            CompleteDialogueEntry();

            _context.Controller.PlayNextScript();
        }

        private async UniTask ProgressCutsceneDialogue()
        {

        }

        private void CompleteDialogueEntry()
        {
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in DialogueHandler.");

            OnStoryEntryComplete.Invoke(this);
            _currentDialogue = null;
        }

        private float CalculateTextInterval(StoryDialogue.TextSpeedType textSpeed)
        {
            switch (textSpeed)
            {
                case StoryDialogue.TextSpeedType.Slow:
                    return 0.05f;
                case StoryDialogue.TextSpeedType.Default:
                    return 0.03f;
                case StoryDialogue.TextSpeedType.Fast:
                    return 0.01f;
                default:
                    return 0.03f; // Default to normal speed
            }
        }
    }
}