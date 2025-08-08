using System;
using Cysharp.Threading.Tasks;

namespace AD.Story
{
    public interface IStoryPresenter
    {
        StoryEntry.EntryType PresentingEntryType { get; }

        event Action<IStoryPresenter> OnStoryEntryComplete;

        void Initialize(StoryController storyController, StoryUIView uiView);
        UniTask ProgressStoryEntry(StoryEntry storyEntry);
        void CompleteStoryEntry();
    }
}