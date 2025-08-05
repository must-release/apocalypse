using UnityEngine;
using UnityEditor;
using StoryEditor.Controllers;

namespace StoryEditor.UI
{
    public class StoryEntryPanel
    {
        private EditorStoryScript editorStoryScript;
        private EntryController entryController;
        private DragDropHelper.DragState dragState;
        private Vector2 scrollPosition;

        public StoryEntryPanel(EditorStoryScript storyScript, EntryController controller)
        {
            editorStoryScript = storyScript;
            entryController = controller;
        }

        public void Draw(Rect rect)
        {
            GUILayout.BeginArea(rect);
            DrawHeader();
            DrawEntryList();
            GUILayout.EndArea();
        }

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
                menu.AddItem(new GUIContent("Dialogue"), false, () => entryController.AddEntry(EntryType.Dialogue));
                menu.AddItem(new GUIContent("VFX"), false, () => entryController.AddEntry(EntryType.VFX));
                menu.AddItem(new GUIContent("Choice"), false, () => entryController.AddEntry(EntryType.Choice));
                menu.DropDown(dropdownRect);
            }
        }

        private void DrawEntryList()
        {
            var selectedBlock = editorStoryScript.SelectedBlock;
            if (null == selectedBlock)
            {
                EditorGUILayout.HelpBox("Select a block to view its entries", MessageType.Info);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (Event.current.type == EventType.Repaint)
            {
                dragState.dropTargetIndex = -1;
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
            var isSelected = index == editorStoryScript.SelectedEntryIndex;

            EditorGUILayout.BeginHorizontal();

            var displayText = $"{index + 1}. {entry.GetDisplayText()}";
            var buttonStyle = isSelected ? GetSelectedButtonStyle() : GUI.skin.label;
            var buttonRect = GUILayoutUtility.GetRect(new GUIContent(displayText), buttonStyle);
            
            var isDraggedItem = dragState.isDragging && dragState.draggedIndex == index;
            
            DragDropHelper.DrawDraggedItemFeedback(isDraggedItem);
            DragDropHelper.DrawDropIndicator(buttonRect, index, ref dragState);
            
            DragDropHelper.HandleDragAndDrop<EditorStoryEntry>(buttonRect, index, ref dragState,
                (fromIndex, toIndex) => entryController.MoveEntry(editorStoryScript.SelectedBlockIndex, fromIndex, toIndex));
            
            if (GUI.Button(buttonRect, displayText, buttonStyle) && false == dragState.isDragging)
            {
                entryController.SelectEntry(index);
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
                    entryController.RemoveEntry(editorStoryScript.SelectedBlockIndex, index);
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

        public void SetDragCursor(Rect windowRect)
        {
            DragDropHelper.SetDragCursor(dragState.isDragging, windowRect);
        }

        public bool IsDragging => dragState.isDragging;
    }
}