using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.Story
{
    public class ChoiceHandler : MonoBehaviour, IStoryEntryHandler
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.Choice;
        public StoryEntry CurrentEntry => _currentChoice;
        public event Action<IStoryEntryHandler> OnStoryEntryComplete;

        public void Initialize(StoryHandleContext context)
        {
            Debug.Assert(null != context, "StoryHandleContext cannot be null in ChoiceHandler.");
            Debug.Assert(context.IsValid, "StoryHandleContext is not valid in ChoiceHandler.");

            _context = context;
            _choicePanel = _context.UIView.ChoicePanel;
            _choicePanel.OnChoiceSelected += ChoiceSelected;

            Debug.Assert(null != _choicePanel, "ChoicePanel is not assigned in ChoiceHandler.");
        }

        public UniTask ProgressStoryEntry(StoryEntry storyEntry)
        {
            Debug.Assert(storyEntry is StoryChoice, $"{storyEntry} is not a StoryChoice");

            _currentChoice = storyEntry as StoryChoice;
            var options = _currentChoice.Options.Select(option => option.Text).ToList();
            _choicePanel.ShowChoices(options.ToArray());

            return UniTask.CompletedTask;
        }

        public void CompleteStoryEntry()
        {
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in ChoiceHandler.");

            if (null == _selectedOption)
                return;

            _choicePanel.HideChoices();
            _context.Controller.ProcessSelectedChoice(_selectedOption);
            OnStoryEntryComplete.Invoke(this);

            // _context.Controller.PlayNextScript();
        }

        public void ResetHandler()
        {
            _currentChoice = null;
            _selectedOption = null;
        }

        /****** Private Members ******/

        private StoryHandleContext  _context;
        private ChoicePanel         _choicePanel;
        private StoryChoice         _currentChoice;
        private StoryChoiceOption   _selectedOption;

        private void ChoiceSelected(string choice)
        {
            Debug.Assert(null != _currentChoice, "Current choice is not set in ChoiceHandler.");

            _selectedOption = _currentChoice.Options.Find(option => option.Text == choice);
            Debug.Assert(null != _selectedOption, $"Selected option '{choice}' not found in current choice options.");

            CompleteStoryEntry();
        }
    }
}