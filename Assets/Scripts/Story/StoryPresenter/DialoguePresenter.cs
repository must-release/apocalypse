using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.Story
{
    public class DialoguePresenter : MonoBehaviour, IStoryEntryHandler
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.Dialogue;
        public event Action<IStoryEntryHandler> OnStoryEntryComplete;

        private StoryContext _context;

        public void Initialize(StoryContext context)
        {
            _context = context;
            Debug.Assert(null != _context.Controller, "StoryController is not assigned in DialoguePresenter context.");
            
            // Access UI elements directly from context
            Debug.Assert(null != _context.UIView, "StoryUIView is not assigned in DialoguePresenter context.");
            
            _dialogueBox = _context.UIView.DialogueBox;
            Debug.Assert(null != _dialogueBox, "DialogueBox is not assigned in DialoguePresenter.");
        }

        public async UniTask ProgressStoryEntry(StoryEntry storyEntry)
        {
            Debug.Assert(storyEntry is StoryDialogue, $"{storyEntry} is not a StoryDialogue");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in DialoguePresenter.");

            _currentDialogue = storyEntry as StoryDialogue;
            _cancellationTokenSource = new CancellationTokenSource();
            _dialogueBox.SetName(_currentDialogue.Name);

            try
            {
                await _dialogueBox.DisplayText(_currentDialogue.Text, CalculateTextInterval(_currentDialogue.TextSpeed), _cancellationTokenSource.Token);
                OnStoryEntryComplete.Invoke(this);
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void CompleteStoryEntry()
        {
            Debug.Assert(null != _currentDialogue, "Current dialogue is null");
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in DialoguePresenter.");

            _cancellationTokenSource?.Cancel();
            _dialogueBox.DisplayText(_currentDialogue.Text);
            OnStoryEntryComplete.Invoke(this);
            _currentDialogue = null;
        }

        /****** Private Members ******/

        // private StoryController         _storyController; // Now accessed via _context.Controller
        private DialogueBox             _dialogueBox;
        private StoryDialogue           _currentDialogue;
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