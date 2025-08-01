using UnityEngine;
using UnityEditor;
using System.IO;
using StoryEditor.Controllers;
using StoryEditor.Serialization;

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

        // UI State
        private Vector2 blockScrollPosition;
        private Vector2 entryScrollPosition;
        private Vector2 editorScrollPosition;
        private string editingBlockName = "";
        private bool isEditingBlockName = false;
        private int editingBlockIndex = -1;
        private bool isDraggingSplitter = false;
        private bool isDraggingRightSplitter = false;
        
        // Drag and Drop State
        private int draggedBlockIndex = -1;
        private int draggedEntryIndex = -1;
        private bool isDraggingBlock = false;
        private bool isDraggingEntry = false;

        // Current file path
        private string currentFilePath = "";

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
            if (editorStoryScript == null)
            {
                editorStoryScript = new EditorStoryScript();
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
        }

        private void OnGUI()
        {
            if (editorStoryScript == null) return;

            DrawMenuBar();
            DrawPanels();
        }

        private void DrawMenuBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("New", EditorStyles.toolbarButton))
            {
                NewFile();
            }

            if (GUILayout.Button("Load", EditorStyles.toolbarButton))
            {
                LoadFile();
            }

            if (GUILayout.Button("Save", EditorStyles.toolbarButton))
            {
                SaveFile();
            }

            if (GUILayout.Button("Save As", EditorStyles.toolbarButton))
            {
                SaveAsFile();
            }

            GUILayout.Space(20);


            GUILayout.FlexibleSpace();

            // Validation status
            var validation = validationController.ValidateAll();
            if (validation.HasErrors)
            {
                GUI.color = Color.red;
                GUILayout.Label($"Errors: {validation.Errors.Count}", EditorStyles.toolbarButton);
                GUI.color = Color.white;
            }
            else if (validation.HasWarnings)
            {
                GUI.color = Color.yellow;
                GUILayout.Label($"Warnings: {validation.Warnings.Count}", EditorStyles.toolbarButton);
                GUI.color = Color.white;
            }
            else
            {
                GUI.color = Color.green;
                GUILayout.Label("Valid", EditorStyles.toolbarButton);
                GUI.color = Color.white;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawPanels()
        {
            var totalRect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            totalRect.y += EditorGUIUtility.singleLineHeight + 2; // Account for toolbar
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

            // Draw panels
            DrawBlockPanel(leftRect);
            DrawEntryPanel(centerRect);
            DrawEditorPanel(rightRect);

            // Draw splitters
            EditorGUI.DrawRect(leftSplitterRect, Color.gray);
            EditorGUI.DrawRect(rightSplitterRect, Color.gray);
        }

        private void HandleSplitterDragging(Rect leftSplitterRect, Rect rightSplitterRect, float totalWidth)
        {
            EditorGUIUtility.AddCursorRect(leftSplitterRect, MouseCursor.ResizeHorizontal);
            EditorGUIUtility.AddCursorRect(rightSplitterRect, MouseCursor.ResizeHorizontal);

            var currentEvent = Event.current;

            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
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

        private void DrawBlockPanel(Rect rect)
        {
            GUILayout.BeginArea(rect);
            GUILayout.Label("Story Blocks", EditorStyles.boldLabel);

            blockScrollPosition = EditorGUILayout.BeginScrollView(blockScrollPosition);

            for (int i = 0; i < editorStoryScript.EditorBlocks.Count; i++)
            {
                var block = editorStoryScript.EditorBlocks[i];
                var isSelected = i == editorStoryScript.SelectedBlockIndex;
                var isDragTarget = isDraggingBlock && draggedBlockIndex != i;

                // Highlight drag target
                if (isDragTarget)
                {
                    var currentEvent = Event.current;
                    var lastRect = GUILayoutUtility.GetLastRect();
                    if (lastRect.Contains(currentEvent.mousePosition))
                    {
                        GUI.color = Color.cyan;
                        GUI.Box(new Rect(0, lastRect.y - 2, rect.width, 4), "");
                        GUI.color = Color.white;
                    }
                }

                EditorGUILayout.BeginHorizontal();

                // Selection and name display/editing
                if (isEditingBlockName && editingBlockIndex == i)
                {
                    editingBlockName = EditorGUILayout.TextField(editingBlockName);
                    
                    if (GUILayout.Button("✓", GUILayout.Width(20)) || 
                        (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return))
                    {
                        blockController.RenameBlock(i, editingBlockName);
                        isEditingBlockName = false;
                        editingBlockIndex = -1;
                        GUI.FocusControl(null);
                    }
                    
                    if (GUILayout.Button("✗", GUILayout.Width(20)) || 
                        (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape))
                    {
                        isEditingBlockName = false;
                        editingBlockIndex = -1;
                        GUI.FocusControl(null);
                    }
                }
                else
                {
                    var displayName = $"{i + 1}. {block.GetDisplayName()}";
                    var buttonStyle = isSelected ? EditorStyles.miniButtonMid : GUI.skin.label;
                    
                    // Handle drag and drop
                    var buttonRect = GUILayoutUtility.GetRect(new GUIContent(displayName), buttonStyle);
                    HandleBlockDragAndDrop(buttonRect, i);
                    
                    if (GUI.Button(buttonRect, displayName, buttonStyle))
                    {
                        if (!isDraggingBlock)
                        {
                            blockController.SelectBlock(i);
                        }
                    }

                    // Double-click to edit name
                    if (Event.current.type == EventType.MouseDown && Event.current.clickCount == 2 && 
                        buttonRect.Contains(Event.current.mousePosition))
                    {
                        editingBlockName = block.BranchId;
                        isEditingBlockName = true;
                        editingBlockIndex = i;
                        Event.current.Use();
                    }

                    // Delete button
                    GUI.color = Color.red;
                    if (GUILayout.Button("✖", GUILayout.Width(20)))
                    {
                        if (EditorUtility.DisplayDialog("Delete Block", "Are you sure you want to delete this block?", "Delete", "Cancel"))
                        {
                            blockController.RemoveBlock(i);
                        }
                    }
                    GUI.color = Color.white;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            // Add block button
            if (GUILayout.Button("Add Block"))
            {
                blockController.AddBlock();
            }

            GUILayout.EndArea();
        }

        private void HandleBlockDragAndDrop(Rect buttonRect, int blockIndex)
        {
            var currentEvent = Event.current;

            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (buttonRect.Contains(currentEvent.mousePosition) && currentEvent.button == 0)
                    {
                        draggedBlockIndex = blockIndex;
                        isDraggingBlock = false; // Don't start dragging until mouse moves
                    }
                    break;

                case EventType.MouseDrag:
                    if (draggedBlockIndex == blockIndex && !isDraggingBlock)
                    {
                        isDraggingBlock = true;
                        currentEvent.Use();
                    }
                    if (isDraggingBlock && draggedBlockIndex != -1)
                    {
                        Repaint();
                    }
                    break;

                case EventType.MouseUp:
                    if (isDraggingBlock && draggedBlockIndex != -1)
                    {
                        // Find the target index based on mouse position
                        for (int i = 0; i < editorStoryScript.EditorBlocks.Count; i++)
                        {
                            if (i != draggedBlockIndex)
                            {
                                var targetRect = GUILayoutUtility.GetLastRect();
                                if (targetRect.Contains(currentEvent.mousePosition))
                                {
                                    blockController.MoveBlock(draggedBlockIndex, i);
                                    break;
                                }
                            }
                        }
                        
                        isDraggingBlock = false;
                        draggedBlockIndex = -1;
                        currentEvent.Use();
                        Repaint();
                    }
                    break;
            }
        }

        private void DrawEntryPanel(Rect rect)
        {
            GUILayout.BeginArea(rect);
            GUILayout.Label("Story Entries", EditorStyles.boldLabel);

            var selectedBlock = editorStoryScript.SelectedBlock;
            if (selectedBlock == null)
            {
                EditorGUILayout.HelpBox("Select a block to view its entries", MessageType.Info);
                GUILayout.EndArea();
                return;
            }

            entryScrollPosition = EditorGUILayout.BeginScrollView(entryScrollPosition);

            for (int i = 0; i < selectedBlock.EditorEntries.Count; i++)
            {
                var entry = selectedBlock.EditorEntries[i];
                var isSelected = i == editorStoryScript.SelectedEntryIndex;
                var isDragTarget = isDraggingEntry && draggedEntryIndex != i;

                // Highlight drag target
                if (isDragTarget)
                {
                    var currentEvent = Event.current;
                    var lastRect = GUILayoutUtility.GetLastRect();
                    if (lastRect.Contains(currentEvent.mousePosition))
                    {
                        GUI.color = Color.cyan;
                        GUI.Box(new Rect(0, lastRect.y - 2, rect.width, 4), "");
                        GUI.color = Color.white;
                    }
                }

                EditorGUILayout.BeginHorizontal();

                var displayText = $"{i + 1}. {entry.GetDisplayText()}";
                var buttonStyle = isSelected ? EditorStyles.miniButtonMid : GUI.skin.label;
                
                // Handle drag and drop
                var buttonRect = GUILayoutUtility.GetRect(new GUIContent(displayText), buttonStyle);
                HandleEntryDragAndDrop(buttonRect, i);
                
                if (GUI.Button(buttonRect, displayText, buttonStyle))
                {
                    if (!isDraggingEntry)
                    {
                        entryController.SelectEntry(i);
                    }
                }

                // Delete button
                GUI.color = Color.red;
                if (GUILayout.Button("✖", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("Delete Entry", "Are you sure you want to delete this entry?", "Delete", "Cancel"))
                    {
                        entryController.RemoveEntry(editorStoryScript.SelectedBlockIndex, i);
                    }
                }
                GUI.color = Color.white;

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            // Add entry controls
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Dialogue"))
            {
                entryController.AddEntry(EntryType.Dialogue);
            }
            
            if (GUILayout.Button("VFX"))
            {
                entryController.AddEntry(EntryType.VFX);
            }
            
            if (GUILayout.Button("Choice"))
            {
                entryController.AddEntry(EntryType.Choice);
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void HandleEntryDragAndDrop(Rect buttonRect, int entryIndex)
        {
            var currentEvent = Event.current;

            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (buttonRect.Contains(currentEvent.mousePosition) && currentEvent.button == 0)
                    {
                        draggedEntryIndex = entryIndex;
                        isDraggingEntry = false; // Don't start dragging until mouse moves
                    }
                    break;

                case EventType.MouseDrag:
                    if (draggedEntryIndex == entryIndex && !isDraggingEntry)
                    {
                        isDraggingEntry = true;
                        currentEvent.Use();
                    }
                    if (isDraggingEntry && draggedEntryIndex != -1)
                    {
                        Repaint();
                    }
                    break;

                case EventType.MouseUp:
                    if (isDraggingEntry && draggedEntryIndex != -1)
                    {
                        var selectedBlock = editorStoryScript.SelectedBlock;
                        if (selectedBlock != null)
                        {
                            // Find the target index based on mouse position
                            for (int i = 0; i < selectedBlock.EditorEntries.Count; i++)
                            {
                                if (i != draggedEntryIndex)
                                {
                                    var targetRect = GUILayoutUtility.GetLastRect();
                                    if (targetRect.Contains(currentEvent.mousePosition))
                                    {
                                        entryController.MoveEntry(editorStoryScript.SelectedBlockIndex, draggedEntryIndex, i);
                                        break;
                                    }
                                }
                            }
                        }
                        
                        isDraggingEntry = false;
                        draggedEntryIndex = -1;
                        currentEvent.Use();
                        Repaint();
                    }
                    break;
            }
        }

        private void DrawEditorPanel(Rect rect)
        {
            GUILayout.BeginArea(rect);
            GUILayout.Label("Entry Editor", EditorStyles.boldLabel);

            var selectedEntry = editorStoryScript.SelectedEntry;
            if (selectedEntry == null)
            {
                EditorGUILayout.HelpBox("Select an entry to edit its properties", MessageType.Info);
                GUILayout.EndArea();
                return;
            }

            editorScrollPosition = EditorGUILayout.BeginScrollView(editorScrollPosition);

            EditorGUILayout.LabelField("Entry Type", selectedEntry.GetEntryType(), EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Entry-specific editors will be implemented in the next step
            switch (selectedEntry.StoryEntry)
            {
                case StoryDialogue dialogue:
                    DrawDialogueEditor(dialogue);
                    break;
                case StoryVFX vfx:
                    DrawVFXEditor(vfx);
                    break;
                case StoryChoice choice:
                    DrawChoiceEditor(choice);
                    break;
            }

            EditorGUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        private void DrawDialogueEditor(StoryDialogue dialogue)
        {
            EditorGUILayout.LabelField("Character:", EditorStyles.boldLabel);
            var characterOptions = new string[] { "독백", "나", "소녀", "중개상" };
            var currentIndex = System.Array.IndexOf(characterOptions, dialogue.Name);
            if (currentIndex == -1) currentIndex = 0;
            
            var newIndex = EditorGUILayout.Popup(currentIndex, characterOptions);
            if (newIndex != currentIndex)
            {
                dialogue.Name = characterOptions[newIndex];
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Text Speed:", EditorStyles.boldLabel);
            dialogue.TextSpeed = (StoryDialogue.TextSpeedType)EditorGUILayout.EnumPopup(dialogue.TextSpeed);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Dialogue Text:", EditorStyles.boldLabel);
            var textStyle = new GUIStyle(EditorStyles.textArea);
            textStyle.wordWrap = true;
            dialogue.Text = EditorGUILayout.TextArea(dialogue.Text ?? "", textStyle, GUILayout.Height(100));
        }

        private void DrawVFXEditor(StoryVFX vfx)
        {
            EditorGUILayout.LabelField("Action:", EditorStyles.boldLabel);
            var actionOptions = new string[] { "FadeIn", "FadeOut", "Shake" };
            var currentIndex = System.Array.IndexOf(actionOptions, vfx.Action);
            if (currentIndex == -1) currentIndex = 0;
            
            var newIndex = EditorGUILayout.Popup(currentIndex, actionOptions);
            if (newIndex != currentIndex)
            {
                vfx.Action = actionOptions[newIndex];
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Duration (seconds):", EditorStyles.boldLabel);
            vfx.Duration = EditorGUILayout.FloatField(vfx.Duration);
            if (vfx.Duration < 0) vfx.Duration = 0;
        }

        private void DrawChoiceEditor(StoryChoice choice)
        {
            EditorGUILayout.LabelField("Previous Dialogue:", EditorStyles.boldLabel);
            if (choice.PrevDialogue != null)
            {
                EditorGUILayout.LabelField($"Character: {choice.PrevDialogue.Name}");
                EditorGUILayout.LabelField($"Text: {choice.PrevDialogue.Text}");
            }
            else
            {
                EditorGUILayout.HelpBox("No previous dialogue found", MessageType.Warning);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Choice Options:", EditorStyles.boldLabel);

            if (choice.Options == null)
            {
                choice.Options = new System.Collections.Generic.List<StoryChoiceOption>();
            }

            for (int i = 0; i < choice.Options.Count; i++)
            {
                var option = choice.Options[i];
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                
                GUILayout.Label($"Option {i + 1}:", EditorStyles.boldLabel, GUILayout.Width(70));
                
                GUI.color = Color.red;
                if (GUILayout.Button("✖", GUILayout.Width(20)))
                {
                    choice.Options.RemoveAt(i);
                    break;
                }
                GUI.color = Color.white;
                
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Branch ID:");
                var availableBranches = validationController.GetAvailableBranchIds(editorStoryScript.SelectedBlockIndex);
                availableBranches.Insert(0, "common");
                
                var branchIndex = availableBranches.IndexOf(option.BranchId ?? "common");
                if (branchIndex == -1) branchIndex = 0;
                
                var newBranchIndex = EditorGUILayout.Popup(branchIndex, availableBranches.ToArray());
                if (newBranchIndex != branchIndex && newBranchIndex >= 0 && newBranchIndex < availableBranches.Count)
                {
                    option.BranchId = availableBranches[newBranchIndex];
                }

                EditorGUILayout.LabelField("Option Text:");
                option.Text = EditorGUILayout.TextField(option.Text ?? "");

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            if (GUILayout.Button("Add Option"))
            {
                choice.Options.Add(new StoryChoiceOption
                {
                    BranchId = "common",
                    Text = ""
                });
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Update Previous Dialogue"))
            {
                var selectedEntry = editorStoryScript.SelectedEntry;
                if (selectedEntry != null)
                {
                    entryController.UpdateChoiceEntry(selectedEntry, 
                        editorStoryScript.SelectedBlockIndex, 
                        editorStoryScript.SelectedEntryIndex);
                }
            }
        }

        #region File Operations

        private void NewFile()
        {
            if (EditorUtility.DisplayDialog("New File", "Create a new story script? Unsaved changes will be lost.", "New", "Cancel"))
            {
                editorStoryScript = new EditorStoryScript();
                InitializeControllers();
                currentFilePath = "";
                Repaint();
            }
        }

        private void LoadFile()
        {
            var path = EditorUtility.OpenFilePanel("Load Story Script", "", "xml");
            if (!string.IsNullOrEmpty(path))
            {
                if (StoryScriptSerializer.LoadFromXml(path, out var loadedScript, out var errorMessage))
                {
                    editorStoryScript = loadedScript;
                    InitializeControllers();
                    currentFilePath = path;
                    Repaint();
                }
                else
                {
                    EditorUtility.DisplayDialog("Load Error", errorMessage, "OK");
                }
            }
        }

        private void SaveFile()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveAsFile();
            }
            else
            {
                if (StoryScriptSerializer.SaveToXml(editorStoryScript, currentFilePath, out var errorMessage))
                {
                    EditorUtility.DisplayDialog("Save Success", "File saved successfully!", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Save Error", errorMessage, "OK");
                }
            }
        }

        private void SaveAsFile()
        {
            var path = EditorUtility.SaveFilePanel("Save Story Script", "", "StoryScript", "xml");
            if (!string.IsNullOrEmpty(path))
            {
                if (StoryScriptSerializer.SaveToXml(editorStoryScript, path, out var errorMessage))
                {
                    currentFilePath = path;
                    EditorUtility.DisplayDialog("Save Success", "File saved successfully!", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Save Error", errorMessage, "OK");
                }
            }
        }


        #endregion
    }
}