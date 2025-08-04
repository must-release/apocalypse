using StoryEditor.Controllers;
using System.Collections.Generic;

namespace StoryEditor.UI
{
    public class StoryEntryEditorFactory
    {
        private ValidationController validationController;
        private EditorStoryScript editorStoryScript;
        private Dictionary<System.Type, IStoryEntryEditor> editorCache;

        public StoryEntryEditorFactory(ValidationController validationController, EditorStoryScript editorStoryScript)
        {
            this.validationController = validationController;
            this.editorStoryScript = editorStoryScript;
            this.editorCache = new Dictionary<System.Type, IStoryEntryEditor>();
        }

        public IStoryEntryEditor GetEditor(EditorStoryEntry entry)
        {
            var entryType = entry.StoryEntry.GetType();
            
            if (editorCache.TryGetValue(entryType, out var cachedEditor))
            {
                return cachedEditor;
            }

            var editor = CreateEditor(entry);
            if (null != editor)
            {
                editorCache[entryType] = editor;
            }
            
            return editor;
        }

        public void NotifyTextRefresh()
        {
            if (editorCache.TryGetValue(typeof(StoryDialogue), out var dialogueEditor))
            {
                if (dialogueEditor is DialogueEditor dialogueEditorInstance)
                {
                    dialogueEditorInstance.SetNeedsTextRefresh(true);
                }
            }
        }

        private IStoryEntryEditor CreateEditor(EditorStoryEntry entry)
        {
            return entry.StoryEntry switch
            {
                StoryDialogue _ => new DialogueEditor(),
                StoryVFX _ => new VFXEditor(),
                StoryChoice _ => new ChoiceEditor(validationController, editorStoryScript),
                StoryCharacterStanding _ => new CharacterStandingEditor(),
                StoryPlayMode _ => new PlayModeEditor(),
                StoryBackgroundCG _ => new BackgroundCGEditor(),
                StoryBGM _ => new BGMEditor(),
                StorySFX _ => new SFXEditor(),
                StoryCameraAction _ => new CameraActionEditor(),
                _ => null
            };
        }
    }
}