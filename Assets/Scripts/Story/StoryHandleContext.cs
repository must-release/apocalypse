using UnityEngine;

namespace AD.Story
{
    public class StoryHandleContext
    {
        public StoryController Controller { get; private set; }
        public StoryUIView UIView { get; private set; }
        public bool IsValid { get; private set; }
        public StoryPlayMode.PlayModeType CurrentPlayMode { get; private set; }

        public StoryHandleContext(StoryController controller, StoryUIView uiView)
        {
            Debug.Assert(null != controller, "StoryController cannot be null in StoryHandleContext.");
            Debug.Assert(null != uiView, "StoryUIView cannot be null in StoryHandleContext.");

            Controller = controller;
            UIView = uiView;
            CurrentPlayMode = StoryPlayMode.PlayModeType.VisualNovel;

            IsValid = true;
        }

        public void SetCurrentPlayMode(StoryPlayMode.PlayModeType playMode)
        {
            Debug.Assert(playMode != StoryPlayMode.PlayModeType.PlayModeTypeCount, "Invalid PlayMode type: " + playMode);
            CurrentPlayMode = playMode;
        }
    }
}
