using UnityEngine;
using UnityEditor;
using AD.Story;
using StoryEditor.Controllers;
using StoryEditor.UI;

namespace StoryEditor
{
    public class StoryScriptEditorWindow : EditorWindow
    {

        /****** Public Members ******/

        [MenuItem("Window/Story Script Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<StoryScriptEditorWindow>();
            window.titleContent = new GUIContent("Story Script Editor");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }


        /****** Private Members ******/

        private const float _MinPanelWidth = 200f;
        private const float _SplitterWidth = 5f;

        [SerializeField] private EditorStoryScript _editorStoryScript;
        [SerializeField] private float _leftPanelWidth = 250f;
        [SerializeField] private float _rightPanelWidth = 350f;

        // Controllers
        private BlockController _blockController;
        private EntryController _entryController;
        private ValidationController _validationController;

        // UI Components
        private StoryEditorToolbar _toolbar;
        private StoryBlockPanel _blockPanel;
        private StoryEntryPanel _entryPanel;
        private StoryEditorPanel _editorPanel;

        // Layout state
        private bool _isDraggingSplitter = false;
        private bool _isDraggingRightSplitter = false;

        private void OnEnable()
        {
            if (null == _editorStoryScript)
            {
                _editorStoryScript = new EditorStoryScript();
                // Add a default block if none exists
                if (0 == _editorStoryScript.EditorBlocks.Count)
                {
                    var defaultBlock = _editorStoryScript.AddBlock("Common");
                    // Add a default dialogue entry to avoid warnings
                    defaultBlock.AddDialogue("독백", "새로운 스토리를 시작하세요.");
                }
            }

            InitializeControllers();
        }

        private void InitializeControllers()
        {
            Debug.Assert(null != _editorStoryScript);

            _blockController = new BlockController(_editorStoryScript);
            _entryController = new EntryController(_editorStoryScript);
            _validationController = new ValidationController(_editorStoryScript);

            _blockController.SetCallbacks(Repaint, Repaint);
            _entryController.SetCallbacks(Repaint, Repaint);

            // Initialize UI components
            _toolbar = new StoryEditorToolbar(_editorStoryScript, _validationController);
            _toolbar.SetCallbacks("", () => {
                _editorStoryScript = new EditorStoryScript();
                InitializeControllers();
                Repaint();
            }, Repaint);

            _blockPanel = new StoryBlockPanel(_editorStoryScript, _blockController);
            _entryPanel = new StoryEntryPanel(_editorStoryScript, _entryController);
            _editorPanel = new StoryEditorPanel(_editorStoryScript, _entryController, _validationController);
        }

        private void OnGUI()
        {
            if (null == _editorStoryScript || null == _toolbar) return;

            // Handle global input for panels
            _blockPanel?.HandleGlobalInput();

            _toolbar.Draw();
            DrawPanels();
        }

        private void DrawPanels()
        {
            var totalRect = position;
            totalRect.x = 0;
            totalRect.y = EditorGUIUtility.singleLineHeight + 2; // Account for toolbar
            totalRect.height -= EditorGUIUtility.singleLineHeight + 2;

            // Calculate panel rects
            var leftRect = new Rect(totalRect.x, totalRect.y, _leftPanelWidth, totalRect.height);
            var leftSplitterRect = new Rect(leftRect.xMax, totalRect.y, _SplitterWidth, totalRect.height);
            
            var rightSplitterRect = new Rect(totalRect.xMax - _rightPanelWidth - _SplitterWidth, totalRect.y, _SplitterWidth, totalRect.height);
            var rightRect = new Rect(rightSplitterRect.xMax, totalRect.y, _rightPanelWidth, totalRect.height);
            
            var centerRect = new Rect(leftSplitterRect.xMax, totalRect.y, 
                rightSplitterRect.x - leftSplitterRect.xMax, totalRect.height);

            // Handle splitter dragging
            HandleSplitterDragging(leftSplitterRect, rightSplitterRect, totalRect.width);

            // Draw panels using components
            _blockPanel?.Draw(leftRect);
            _entryPanel?.Draw(centerRect);
            _editorPanel?.Draw(rightRect);

            // Draw splitters
            EditorGUI.DrawRect(leftSplitterRect, Color.gray);
            EditorGUI.DrawRect(rightSplitterRect, Color.gray);
        }

        private void HandleSplitterDragging(Rect leftSplitterRect, Rect rightSplitterRect, float totalWidth)
        {
            EditorGUIUtility.AddCursorRect(leftSplitterRect, MouseCursor.ResizeHorizontal);
            EditorGUIUtility.AddCursorRect(rightSplitterRect, MouseCursor.ResizeHorizontal);

            var currentEvent = Event.current;

            if (EventType.MouseDown == currentEvent.type && 0 == currentEvent.button)
            {
                if (leftSplitterRect.Contains(currentEvent.mousePosition))
                {
                    _isDraggingSplitter = true;
                    _isDraggingRightSplitter = false;
                    currentEvent.Use();
                }
                else if (rightSplitterRect.Contains(currentEvent.mousePosition))
                {
                    _isDraggingRightSplitter = true;
                    _isDraggingSplitter = false;
                    currentEvent.Use();
                }
            }

            if (EventType.MouseDrag == currentEvent.type)
            {
                if (_isDraggingSplitter)
                {
                    _leftPanelWidth = Mathf.Clamp(currentEvent.mousePosition.x, _MinPanelWidth, 
                        totalWidth - _rightPanelWidth - _MinPanelWidth - _SplitterWidth * 2);
                    Repaint();
                    currentEvent.Use();
                }
                else if (_isDraggingRightSplitter)
                {
                    _rightPanelWidth = Mathf.Clamp(totalWidth - currentEvent.mousePosition.x - _SplitterWidth, 
                        _MinPanelWidth, totalWidth - _leftPanelWidth - _MinPanelWidth - _SplitterWidth * 2);
                    Repaint();
                    currentEvent.Use();
                }
            }

            if (EventType.MouseUp == currentEvent.type)
            {
                _isDraggingSplitter = false;
                _isDraggingRightSplitter = false;
            }
        }
    }
}