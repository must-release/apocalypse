using System;
using Cysharp.Threading.Tasks;

namespace AD.Story
{
    public interface IStoryEntryHandler
    {
        StoryEntry.EntryType PresentingEntryType { get; }
        event Action<IStoryEntryHandler> OnStoryEntryComplete;
        void Initialize(StoryHandleContext context);
        UniTask ProgressStoryEntry(StoryEntry storyEntry);
        void CompleteStoryEntry();
    }
}