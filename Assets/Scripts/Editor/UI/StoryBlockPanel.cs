using UnityEngine;
using UnityEditor;
using AD.Story;
using StoryEditor.Controllers;

namespace StoryEditor.UI
{
    public class StoryBlockPanel
    {
        /****** Public Members ******/

        public StoryBlockPanel(EditorStoryScript storyScript, BlockController controller)
        {
            Debug.Assert(null != storyScript);
            Debug.Assert(null != controller);

            _editorStoryScript = storyScript;
            _blockController = controller;
        }

        public void Draw(Rect rect)
        {
            GUILayout.BeginArea(rect);
            DrawHeader();
            DrawBlockList();
            GUILayout.EndArea();
        }

        public void HandleGlobalInput()
        {
            if (_isEditingBlockName && EventType.MouseDown == Event.current.type && 0 == Event.current.button)
            {
                _blockController.RenameBlock(_editingBlockIndex, _editingBlockName);
                ExitEditMode();
            }
        }
        

        /****** Private Members ******/

        private EditorStoryScript _editorStoryScript;
        private BlockController _blockController;
        private DragDropHelper.DragState _dragState;
        private Vector2 _scrollPosition;
        
        private string _editingBlockName = "";
        private bool _isEditingBlockName = false;
        private int _editingBlockIndex = -1;
        private const string _TextFieldControlName = "BlockNameField";

        private void DrawHeader()
        {
            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Story Blocks", EditorStyles.boldLabel);
            if (GUILayout.Button("+ Add", EditorStyles.miniButton, GUILayout.Width(50), GUILayout.Height(16)))
            {
                _blockController.AddBlock();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(3);
        }

        private void DrawBlockList()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            if (EventType.Repaint == Event.current.type)
            {
                _dragState.dropTargetIndex = -1;
            }

            for (int i = 0; i < _editorStoryScript.EditorBlocks.Count; i++)
            {
                DrawBlockItem(i);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawBlockItem(int index)
        {
            Debug.Assert(0 <= index && index < _editorStoryScript.EditorBlocks.Count);

            var block = _editorStoryScript.EditorBlocks[index];
            var isSelected = index == _editorStoryScript.SelectedBlockIndex;

            EditorGUILayout.BeginHorizontal();

            if (_isEditingBlockName && _editingBlockIndex == index)
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
            GUI.SetNextControlName(_TextFieldControlName);
            _editingBlockName = EditorGUILayout.TextField(_editingBlockName);
            
            if (EventType.Repaint == Event.current.type && _TextFieldControlName != GUI.GetNameOfFocusedControl())
            {
                EditorGUI.FocusTextInControl(_TextFieldControlName);
            }
            
            HandleEditingKeys(index);
        }

        private void HandleEditingKeys(int index)
        {
            if (EventType.KeyDown == Event.current.type)
            {
                if (KeyCode.Return == Event.current.keyCode)
                {
                    _blockController.RenameBlock(index, _editingBlockName);
                    ExitEditMode();
                    Event.current.Use();
                }
                else if (KeyCode.Escape == Event.current.keyCode)
                {
                    ExitEditMode();
                    Event.current.Use();
                }
            }
        }

        private void DrawBlockButton(EditorStoryBlock block, int index, bool isSelected)
        {
            Debug.Assert(null != block);

            var displayName = $"{index + 1}. {block.GetDisplayName()}";
            var buttonStyle = isSelected ? GetSelectedButtonStyle() : GUI.skin.label;
            var buttonRect = GUILayoutUtility.GetRect(new GUIContent(displayName), buttonStyle);
            
            var isDraggedItem = _dragState.isDragging && _dragState.draggedIndex == index;
            
            DragDropHelper.DrawDraggedItemFeedback(isDraggedItem);
            DragDropHelper.DrawDropIndicator(buttonRect, index, ref _dragState);
            
            DragDropHelper.HandleDragAndDrop<EditorStoryBlock>(buttonRect, index, ref _dragState, 
                (fromIndex, toIndex) => _blockController.MoveBlock(fromIndex, toIndex));
            
            HandleDoubleClick(buttonRect, block, index);
            
            if (GUI.Button(buttonRect, displayName, buttonStyle) && false == _dragState.isDragging)
            {
                _blockController.SelectBlock(index);
            }
            
            DragDropHelper.ResetGUIColor();
        }

        private void HandleDoubleClick(Rect buttonRect, EditorStoryBlock block, int index)
        {
            if (EventType.MouseDown == Event.current.type && 
                2 == Event.current.clickCount && 
                buttonRect.Contains(Event.current.mousePosition))
            {
                _editingBlockName = block.BranchName;
                _isEditingBlockName = true;
                _editingBlockIndex = index;
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
                    _blockController.RemoveBlock(index);
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

        private void ExitEditMode()
        {
            _isEditingBlockName = false;
            _editingBlockIndex = -1;
            GUI.FocusControl(null);
        }
    }
}