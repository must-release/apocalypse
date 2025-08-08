using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.Story
{
    public class ChoicePresenter : MonoBehaviour, IStoryPresenter
    {
        /****** Public Members ******/

        public StoryEntry.EntryType PresentingEntryType => StoryEntry.EntryType.Choice;
        public event Action<IStoryPresenter> OnStoryEntryComplete;

        public void Initialize(StoryController storyController, StoryUIView uiView)
        {
            _storyController = storyController;
            _choicePanel = uiView.ChoicePanel;
            _choicePanel.OnChoiceSelected += ChoiceSelected;

            Debug.Assert(null != _choicePanel, "ChoicePanel is not assigned in ChoicePresenter.");
            Debug.Assert(null != _storyController, "StoryController is not assigned in ChoicePresenter.");
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
            Debug.Assert(null != OnStoryEntryComplete, "OnStoryEntryComplete event is not subscribed in ChoicePresenter.");

            if (null == _selectedOption)
                return;

            _choicePanel.HideChoices();
            _storyController.ProcessSelectedChoice(_selectedOption);
            _selectedOption = null;
            OnStoryEntryComplete.Invoke(this);

            _storyController.PlayNextScript();
        }

        /****** Private Members ******/

        private StoryController     _storyController;
        private ChoicePanel         _choicePanel;
        private StoryChoice         _currentChoice;
        private StoryChoiceOption   _selectedOption;

        private void ChoiceSelected(string choice)
        {
            Debug.Assert(null != _currentChoice, "Current choice is not set in ChoicePresenter.");

            _selectedOption = _currentChoice.Options.Find(option => option.Text == choice);
            Debug.Assert(null != _selectedOption, $"Selected option '{choice}' not found in current choice options.");

            CompleteStoryEntry();
        }
    }
}