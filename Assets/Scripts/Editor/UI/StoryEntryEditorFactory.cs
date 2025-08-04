using StoryEditor.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace StoryEditor.UI
{
    public class StoryEntryEditorFactory
    {
        /****** Public Members ******/

        public StoryEntryEditorFactory(ValidationController validationController, EditorStoryScript editorStoryScript)
        {
            Debug.Assert(null != validationController);
            Debug.Assert(null != editorStoryScript);

            this._validationController = validationController;
            this._editorStoryScript = editorStoryScript;
            this._editorCache = new Dictionary<System.Type, IStoryEntryEditor>();
        }

        public IStoryEntryEditor GetEditor(EditorStoryEntry entry)
        {
            Debug.Assert(null != entry);

            var entryType = entry.StoryEntry.GetType();
            
            if (_editorCache.TryGetValue(entryType, out var cachedEditor))
            {
                return cachedEditor;
            }

            var editor = CreateEditor(entry);
            if (null != editor)
            {
                _editorCache[entryType] = editor;
            }
            
            return editor;
        }

        public void NotifyTextRefresh()
        {
            if (_editorCache.TryGetValue(typeof(StoryDialogue), out var dialogueEditor))
            {
                if (dialogueEditor is DialogueEditor dialogueEditorInstance)
                {
                    dialogueEditorInstance.SetNeedsTextRefresh(true);
                }
            }
        }


        /****** Private Members ******/

        private ValidationController _validationController;
        private EditorStoryScript _editorStoryScript;
        private Dictionary<System.Type, IStoryEntryEditor> _editorCache;

        private IStoryEntryEditor CreateEditor(EditorStoryEntry entry)
        {
            Debug.Assert(null != entry);

            return entry.StoryEntry switch
            {
                StoryDialogue _ => new DialogueEditor(),
                StoryVFX _ => new VFXEditor(),
                StoryChoice _ => new ChoiceEditor(_validationController, _editorStoryScript),
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