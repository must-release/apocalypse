using System;
using Cysharp.Threading.Tasks;

namespace AD.Story
{
    public interface IStoryPresenter
    {
        void Initialize(StoryController storyController, StoryUIView uiView);
        UniTask ProgressStoryEntry(StoryEntry storyEntry);
    }
}