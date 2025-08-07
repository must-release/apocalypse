using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.Story
{
    public class DialoguePresenter : MonoBehaviour, IStoryPresenter
    {
        /****** Public Members ******/

        public void Initialize(StoryController storyController, StoryUIView uiView)
        {
            _storyController = storyController;
            _dialogueBox = uiView.DialogueBox;

            Debug.Assert(null != _dialogueBox, "DialogueBox is not assigned in DialoguePresenter.");
            Debug.Assert(null != _storyController, "StoryController is not assigned in DialoguePresenter.");
        }

        public async UniTask ProgressStoryEntry(StoryEntry storyEntry)
        {
            Debug.Assert(storyEntry is StoryDialogue, $"{storyEntry} is not a StoryDialogue");
            var dialogue = storyEntry as StoryDialogue;

            _dialogueBox.SetName(dialogue.Name);
            //await _dialogueBox.DisplayText(dialogue.Text, dialogue.TextSpeed);
        }

        /****** Private Members ******/

        private StoryController _storyController;
        private DialogueBox _dialogueBox;
    }
}