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
        public StoryEntry CurrentEntry => _currentDialogue;
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
            _dialogueBox.SetName(_currentDialogue.Name);

            try
            {
                await _dialogueBox.DisplayText(_currentDialogue.Text, CalculateTextInterval(_currentDialogue.TextSpeed), _cancellationTokenSource.Token);
                CompleteStoryEntry();
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void CompleteStoryEntry()
        {
            Debug.Assert(null != _currentDialogue, "Current dialogue is null");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in DialogueHandler.");

            _cancellationTokenSource?.Cancel();
            _dialogueBox.DisplayText(_currentDialogue.Text);
            OnStoryEntryComplete.Invoke(this);
        }

        public void ResetHandler()
        {
            _currentDialogue = null;
        }


        /****** Private Members ******/

        private StoryHandleContext _context;
        private StoryDialogue           _currentDialogue;
        private DialogueBox _dialogueBox;
        private CancellationTokenSource _cancellationTokenSource;

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