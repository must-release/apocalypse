using UnityEngine;
using UnityEditor;
using StoryEditor.Controllers;

namespace StoryEditor.UI
{
    public class StoryEntryPanel
    {
        /****** Public Members ******/

        public StoryEntryPanel(EditorStoryScript storyScript, EntryController controller)
        {
            Debug.Assert(null != storyScript, "Story script cannot be null");
            Debug.Assert(null != controller, "Controller cannot be null");
            
            _editorStoryScript = storyScript;
            _entryController = controller;
        }

        public void Draw(Rect rect)
        {
            GUILayout.BeginArea(rect);
            DrawHeader();
            DrawEntryList();
            GUILayout.EndArea();
        }


        /****** Private Members ******/

        private EditorStoryScript _editorStoryScript;
        private EntryController _entryController;
        private DragDropHelper.DragState _dragState;
        private Vector2 _scrollPosition;

        private void DrawHeader()
        {
            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Story Entries", EditorStyles.boldLabel);
            DrawAddEntryDropdown();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(3);
        }

        private void DrawAddEntryDropdown()
        {
            var dropdownRect = GUILayoutUtility.GetRect(new GUIContent("+ Add"), EditorStyles.miniPullDown, GUILayout.Width(60), GUILayout.Height(16));
            if (GUI.Button(dropdownRect, "+ Add", EditorStyles.miniPullDown))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Dialogue"), false, () => _entryController.AddEntry(StoryEntry.EntryType.Dialogue));
                menu.AddItem(new GUIContent("VFX"), false, () => _entryController.AddEntry(StoryEntry.EntryType.VFX));
                menu.AddItem(new GUIContent("Choice"), false, () => _entryController.AddEntry(StoryEntry.EntryType.Choice));
                menu.AddItem(new GUIContent("CharacterCG"), false, () => _entryController.AddEntry(StoryEntry.EntryType.CharacterCG));
                menu.AddItem(new GUIContent("PlayMode"), false, () => _entryController.AddEntry(StoryEntry.EntryType.PlayMode));
                menu.AddItem(new GUIContent("BackgroundCG"), false, () => _entryController.AddEntry(StoryEntry.EntryType.BackgroundCG));
                menu.AddItem(new GUIContent("BGM"), false, () => _entryController.AddEntry(StoryEntry.EntryType.BGM));
                menu.AddItem(new GUIContent("SFX"), false, () => _entryController.AddEntry(StoryEntry.EntryType.SFX));
                menu.AddItem(new GUIContent("Camera Action"), false, () => _entryController.AddEntry(StoryEntry.EntryType.CameraAction));
                menu.DropDown(dropdownRect);
            }
        }

        private void DrawEntryList()
        {
            var selectedBlock = _editorStoryScript.SelectedBlock;
            if (null == selectedBlock)
            {
                EditorGUILayout.HelpBox("Select a block to view its entries", MessageType.Info);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            if (Event.current.type == EventType.Repaint)
            {
                _dragState.dropTargetIndex = -1;
            }

            for (int i = 0; i < selectedBlock.EditorEntries.Count; i++)
            {
                DrawEntryItem(selectedBlock, i);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawEntryItem(EditorStoryBlock selectedBlock, int index)
        {
            var entry = selectedBlock.EditorEntries[index];
            var isSelected = index == _editorStoryScript.SelectedEntryIndex;

            EditorGUILayout.BeginHorizontal();

            var displayText = $"{index + 1}. {entry.GetDisplayText()}";
            var buttonStyle = isSelected ? GetSelectedButtonStyle() : GUI.skin.label;
            var buttonRect = GUILayoutUtility.GetRect(new GUIContent(displayText), buttonStyle);
            
            var isDraggedItem = _dragState.isDragging && _dragState.draggedIndex == index;
            
            DragDropHelper.DrawDraggedItemFeedback(isDraggedItem);
            DragDropHelper.DrawDropIndicator(buttonRect, index, ref _dragState);
            
            DragDropHelper.HandleDragAndDrop<EditorStoryEntry>(buttonRect, index, ref _dragState,
                (fromIndex, toIndex) => _entryController.MoveEntry(_editorStoryScript.SelectedBlockIndex, fromIndex, toIndex));
            
            if (GUI.Button(buttonRect, displayText, buttonStyle) && false == _dragState.isDragging)
            {
                _entryController.SelectEntry(index);
            }
            
            DragDropHelper.ResetGUIColor();
            DrawDeleteButton(index);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawDeleteButton(int index)
        {
            GUI.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            if (GUILayout.Button("✖", GUILayout.Width(20)))
            {
                if (EditorUtility.DisplayDialog("Delete Entry", "Are you sure you want to delete this entry?", "Delete", "Cancel"))
                {
                    _entryController.RemoveEntry(_editorStoryScript.SelectedBlockIndex, index);
                }
            }
            GUI.color = Color.white;
        }

        private GUIStyle GetSelectedButtonStyle()
        {
            var style = new GUIStyle(EditorStyles.miniButtonMid);
            style.alignment = TextAnchor.MiddleLeft;
            return style;
        }
    }
}