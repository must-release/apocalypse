using UnityEngine;
using UnityEditor;

namespace StoryEditor.UI
{
    public static class DragDropHelper
    {
        public struct DragState
        {
            public int draggedIndex;
            public int dropTargetIndex;
            public bool isDragging;
            public bool insertAbove;
            
            public void Reset()
            {
                draggedIndex = -1;
                dropTargetIndex = -1;
                isDragging = false;
                insertAbove = false;
            }
        }

        public static void HandleDragAndDrop<T>(Rect itemRect, int itemIndex, ref DragState dragState, System.Action<int, int> onMove)
        {
            var currentEvent = Event.current;

            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (itemRect.Contains(currentEvent.mousePosition) && 0 == currentEvent.button)
                    {
                        dragState.draggedIndex = itemIndex;
                        dragState.isDragging = false;
                    }
                    break;

                case EventType.MouseDrag:
                    if (dragState.draggedIndex == itemIndex && false == dragState.isDragging)
                    {
                        dragState.isDragging = true;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (dragState.isDragging && dragState.draggedIndex == itemIndex)
                    {
                        if (dragState.dropTargetIndex != -1 && dragState.dropTargetIndex != dragState.draggedIndex)
                        {
                            int finalTargetIndex = dragState.dropTargetIndex;
                            
                            if (false == dragState.insertAbove)
                            {
                                finalTargetIndex++;
                            }
                            
                            if (dragState.draggedIndex < finalTargetIndex)
                            {
                                finalTargetIndex--;
                            }
                            
                            onMove?.Invoke(dragState.draggedIndex, finalTargetIndex);
                        }
                        
                        dragState.Reset();
                        currentEvent.Use();
                    }
                    break;
            }
        }

        public static void DrawDropIndicator(Rect itemRect, int itemIndex, ref DragState dragState)
        {
            if (false == dragState.isDragging || dragState.draggedIndex == itemIndex) 
                return;

            if (false == itemRect.Contains(Event.current.mousePosition)) 
                return;

            dragState.dropTargetIndex = itemIndex;
            
            float mouseY = Event.current.mousePosition.y;
            float rectCenterY = itemRect.y + itemRect.height * 0.5f;
            dragState.insertAbove = mouseY < rectCenterY;
            
            Rect lineRect = dragState.insertAbove 
                ? new Rect(itemRect.x, itemRect.y - 1, itemRect.width, 2)
                : new Rect(itemRect.x, itemRect.yMax - 1, itemRect.width, 2);
                
            EditorGUI.DrawRect(lineRect, new Color(0.2f, 0.6f, 1f, 1f));
        }

        public static void DrawDraggedItemFeedback(bool isDraggedItem)
        {
            if (isDraggedItem)
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
            }
        }

        public static void ResetGUIColor()
        {
            GUI.color = Color.white;
        }

        public static void SetDragCursor(bool isDragging, Rect windowRect)
        {
            // Mouse cursor change functionality disabled
        }
    }
}