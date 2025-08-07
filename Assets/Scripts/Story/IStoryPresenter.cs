using System;
using Cysharp.Threading.Tasks;

public interface IStoryPresenter
{
    void Initialize(StoryController storyController, StoryUIView uiView);
    UniTask ProgressStoryEntry(StoryEntry storyEntry);
}