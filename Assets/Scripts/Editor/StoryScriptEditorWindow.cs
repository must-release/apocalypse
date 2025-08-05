using UnityEngine;
using UnityEditor;
using StoryEditor.Controllers;
using StoryEditor.UI;

namespace StoryEditor
{
    public class StoryScriptEditorWindow : EditorWindow
    {
        private const float MIN_PANEL_WIDTH = 200f;
        private const float SPLITTER_WIDTH = 5f;

        [SerializeField] private EditorStoryScript editorStoryScript;
        [SerializeField] private float leftPanelWidth = 250f;
        [SerializeField] private float rightPanelWidth = 350f;

        // Controllers
        private BlockController blockController;
        private EntryController entryController;
        private ValidationController validationController;

        // UI Components
        private StoryEditorToolbar toolbar;
        private StoryBlockPanel blockPanel;
        private StoryEntryPanel entryPanel;
        private StoryEditorPanel editorPanel;

        // Layout state
        private bool isDraggingSplitter = false;
        private bool isDraggingRightSplitter = false;

        [MenuItem("Window/Story Script Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<StoryScriptEditorWindow>();
            window.titleContent = new GUIContent("Story Script Editor");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }

        private void OnEnable()
        {
            if (null == editorStoryScript)
            {
                editorStoryScript = new EditorStoryScript();
                // Add a default block if none exists
                if (editorStoryScript.EditorBlocks.Count == 0)
                {
                    var defaultBlock = editorStoryScript.AddBlock("Common");
                    // Add a default dialogue entry to avoid warnings
                    defaultBlock.AddDialogue("독백", "새로운 스토리를 시작하세요.");
                }
            }

            InitializeControllers();
        }

        private void InitializeControllers()
        {
            blockController = new BlockController(editorStoryScript);
            entryController = new EntryController(editorStoryScript);
            validationController = new ValidationController(editorStoryScript);

            blockController.SetCallbacks(Repaint, Repaint);
            entryController.SetCallbacks(Repaint, Repaint);

            // Initialize UI components
            toolbar = new StoryEditorToolbar(editorStoryScript, validationController);
            toolbar.SetCallbacks("", () => {
                editorStoryScript = new EditorStoryScript();
                InitializeControllers();
                Repaint();
            }, Repaint);

            blockPanel = new StoryBlockPanel(editorStoryScript, blockController);
            entryPanel = new StoryEntryPanel(editorStoryScript, entryController);
            editorPanel = new StoryEditorPanel(editorStoryScript, entryController, validationController);
        }

        private void OnGUI()
        {
            if (null == editorStoryScript || null == toolbar) return;

            // Handle global input for panels
            blockPanel?.HandleGlobalInput();

            // Drag cursor functionality disabled

            toolbar.Draw();
            DrawPanels();
        }

        private void DrawPanels()
        {
            var totalRect = position;
            totalRect.x = 0;
            totalRect.y = EditorGUIUtility.singleLineHeight + 2; // Account for toolbar
            totalRect.height -= EditorGUIUtility.singleLineHeight + 2;

            // Calculate panel rects
            var leftRect = new Rect(totalRect.x, totalRect.y, leftPanelWidth, totalRect.height);
            var leftSplitterRect = new Rect(leftRect.xMax, totalRect.y, SPLITTER_WIDTH, totalRect.height);
            
            var rightSplitterRect = new Rect(totalRect.xMax - rightPanelWidth - SPLITTER_WIDTH, totalRect.y, SPLITTER_WIDTH, totalRect.height);
            var rightRect = new Rect(rightSplitterRect.xMax, totalRect.y, rightPanelWidth, totalRect.height);
            
            var centerRect = new Rect(leftSplitterRect.xMax, totalRect.y, 
                rightSplitterRect.x - leftSplitterRect.xMax, totalRect.height);

            // Handle splitter dragging
            HandleSplitterDragging(leftSplitterRect, rightSplitterRect, totalRect.width);

            // Draw panels using components
            blockPanel?.Draw(leftRect);
            entryPanel?.Draw(centerRect);
            editorPanel?.Draw(rightRect);

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
                    isDraggingSplitter = true;
                    isDraggingRightSplitter = false;
                    currentEvent.Use();
                }
                else if (rightSplitterRect.Contains(currentEvent.mousePosition))
                {
                    isDraggingRightSplitter = true;
                    isDraggingSplitter = false;
                    currentEvent.Use();
                }
            }

            if (currentEvent.type == EventType.MouseDrag)
            {
                if (isDraggingSplitter)
                {
                    leftPanelWidth = Mathf.Clamp(currentEvent.mousePosition.x, MIN_PANEL_WIDTH, 
                        totalWidth - rightPanelWidth - MIN_PANEL_WIDTH - SPLITTER_WIDTH * 2);
                    Repaint();
                    currentEvent.Use();
                }
                else if (isDraggingRightSplitter)
                {
                    rightPanelWidth = Mathf.Clamp(totalWidth - currentEvent.mousePosition.x - SPLITTER_WIDTH, 
                        MIN_PANEL_WIDTH, totalWidth - leftPanelWidth - MIN_PANEL_WIDTH - SPLITTER_WIDTH * 2);
                    Repaint();
                    currentEvent.Use();
                }
            }

            if (currentEvent.type == EventType.MouseUp)
            {
                isDraggingSplitter = false;
                isDraggingRightSplitter = false;
            }
        }
    }
}