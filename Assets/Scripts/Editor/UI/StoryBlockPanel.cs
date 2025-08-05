using UnityEngine;
using UnityEditor;
using StoryEditor.Controllers;

namespace StoryEditor.UI
{
    public class StoryBlockPanel
    {
        private EditorStoryScript editorStoryScript;
        private BlockController blockController;
        private DragDropHelper.DragState dragState;
        private Vector2 scrollPosition;
        
        private string editingBlockName = "";
        private bool isEditingBlockName = false;
        private int editingBlockIndex = -1;
        private const string TextFieldControlName = "BlockNameField";

        public StoryBlockPanel(EditorStoryScript storyScript, BlockController controller)
        {
            editorStoryScript = storyScript;
            blockController = controller;
        }

        public void Draw(Rect rect)
        {
            GUILayout.BeginArea(rect);
            DrawHeader();
            DrawBlockList();
            GUILayout.EndArea();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Story Blocks", EditorStyles.boldLabel);
            if (GUILayout.Button("+ Add", EditorStyles.miniButton, GUILayout.Width(50), GUILayout.Height(16)))
            {
                blockController.AddBlock();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(3);
        }

        private void DrawBlockList()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (Event.current.type == EventType.Repaint)
            {
                dragState.dropTargetIndex = -1;
            }

            for (int i = 0; i < editorStoryScript.EditorBlocks.Count; i++)
            {
                DrawBlockItem(i);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawBlockItem(int index)
        {
            var block = editorStoryScript.EditorBlocks[index];
            var isSelected = index == editorStoryScript.SelectedBlockIndex;

            EditorGUILayout.BeginHorizontal();

            if (isEditingBlockName && editingBlockIndex == index)
            {
                DrawBlockNameEditor(index);
            }
            else
            {
                DrawBlockButton(block, index, isSelected);
            }

            DrawDeleteButton(index);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawBlockNameEditor(int index)
        {
            GUI.SetNextControlName(TextFieldControlName);
            editingBlockName = EditorGUILayout.TextField(editingBlockName);
            
            if (Event.current.type == EventType.Repaint && GUI.GetNameOfFocusedControl() != TextFieldControlName)
            {
                EditorGUI.FocusTextInControl(TextFieldControlName);
            }
            
            HandleEditingKeys(index);
        }

        private void HandleEditingKeys(int index)
        {
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Return)
                {
                    blockController.RenameBlock(index, editingBlockName);
                    ExitEditMode();
                    Event.current.Use();
                }
                else if (Event.current.keyCode == KeyCode.Escape)
                {
                    ExitEditMode();
                    Event.current.Use();
                }
            }
        }

        private void DrawBlockButton(EditorStoryBlock block, int index, bool isSelected)
        {
            var displayName = $"{index + 1}. {block.GetDisplayName()}";
            var buttonStyle = isSelected ? GetSelectedButtonStyle() : GUI.skin.label;
            var buttonRect = GUILayoutUtility.GetRect(new GUIContent(displayName), buttonStyle);
            
            var isDraggedItem = dragState.isDragging && dragState.draggedIndex == index;
            
            DragDropHelper.DrawDraggedItemFeedback(isDraggedItem);
            DragDropHelper.DrawDropIndicator(buttonRect, index, ref dragState);
            
            DragDropHelper.HandleDragAndDrop<EditorStoryBlock>(buttonRect, index, ref dragState, 
                (fromIndex, toIndex) => blockController.MoveBlock(fromIndex, toIndex));
            
            HandleDoubleClick(buttonRect, block, index);
            
            if (GUI.Button(buttonRect, displayName, buttonStyle) && false == dragState.isDragging)
            {
                blockController.SelectBlock(index);
            }
            
            DragDropHelper.ResetGUIColor();
        }

        private void HandleDoubleClick(Rect buttonRect, EditorStoryBlock block, int index)
        {
            if (Event.current.type == EventType.MouseDown && 
                Event.current.clickCount == 2 && 
                buttonRect.Contains(Event.current.mousePosition))
            {
                editingBlockName = block.BranchName;
                isEditingBlockName = true;
                editingBlockIndex = index;
                Event.current.Use();
            }
        }

        private void DrawDeleteButton(int index)
        {
            GUI.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            if (GUILayout.Button("✖", GUILayout.Width(20)))
            {
                if (EditorUtility.DisplayDialog("Delete Block", "Are you sure you want to delete this block?", "Delete", "Cancel"))
                {
                    blockController.RemoveBlock(index);
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

        public void HandleGlobalInput()
        {
            if (isEditingBlockName && Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                blockController.RenameBlock(editingBlockIndex, editingBlockName);
                ExitEditMode();
            }
        }

        private void ExitEditMode()
        {
            isEditingBlockName = false;
            editingBlockIndex = -1;
            GUI.FocusControl(null);
        }

        public void SetDragCursor(Rect windowRect)
        {
            DragDropHelper.SetDragCursor(dragState.isDragging, windowRect);
        }

        public bool IsDragging => dragState.isDragging;
    }
}