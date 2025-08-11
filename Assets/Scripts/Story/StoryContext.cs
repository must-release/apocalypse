using UnityEngine;

namespace AD.Story
{
    public class StoryContext
    {
        public StoryController Controller { get; private set; }
        public StoryUIView UIView { get; private set; }

        public StoryContext(StoryController controller, StoryUIView uiView)
        {
            Controller = controller;
            UIView = uiView;
        }
    }
}
