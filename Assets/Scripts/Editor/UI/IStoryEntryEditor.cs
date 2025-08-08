using UnityEngine;
using AD.Story;

namespace StoryEditor.UI
{
    public interface IStoryEntryEditor
    {
        void Draw(EditorStoryEntry entry);
    }
}