using UnityEngine;
using UnityEditor;

namespace StoryEditor.UI
{
    public class CameraActionEditor : IStoryEntryEditor
    {
        /****** Public Members ******/

        public void Draw(EditorStoryEntry entry)
        {
            var cameraAction = entry.AsCameraAction();
            if (null == cameraAction) return;

            DrawCameraActionEditor(cameraAction);
        }


        /****** Private Members ******/

        private void DrawCameraActionEditor(StoryCameraAction cameraAction)
        {
            EditorGUILayout.LabelField("Camera Action Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawActionTypeField(cameraAction);
            EditorGUILayout.Space();

            // Draw fields based on action type
            switch (cameraAction.ActionType)
            {
                case StoryCameraAction.CameraActionType.SwitchToCamera:
                    DrawCameraNameField(cameraAction);
                    break;

                case StoryCameraAction.CameraActionType.FollowTarget:
                    DrawCameraNameField(cameraAction);
                    DrawTargetNameField(cameraAction);
                    break;

                case StoryCameraAction.CameraActionType.SetPriority:
                    DrawCameraNameField(cameraAction);
                    DrawPriorityField(cameraAction);
                    break;

                case StoryCameraAction.CameraActionType.Zoom:
                    DrawCameraNameField(cameraAction);
                    DrawFieldOfViewField(cameraAction);
                    DrawDurationField(cameraAction);
                    DrawEaseTypeField(cameraAction);
                    break;

                case StoryCameraAction.CameraActionType.MoveTo:
                    DrawCameraNameField(cameraAction);
                    DrawPositionFields(cameraAction);
                    DrawDurationField(cameraAction);
                    DrawEaseTypeField(cameraAction);
                    break;

                case StoryCameraAction.CameraActionType.Shake:
                    DrawCameraNameField(cameraAction);
                    DrawIntensityField(cameraAction);
                    DrawDurationField(cameraAction);
                    break;

                case StoryCameraAction.CameraActionType.ResetToDefault:
                    DrawCameraNameField(cameraAction);
                    DrawDurationField(cameraAction);
                    break;
            }

            EditorGUILayout.Space();
            DrawWaitForCompletionField(cameraAction);
        }

        private void DrawActionTypeField(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Action Type:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var newActionType = (StoryCameraAction.CameraActionType)EditorGUILayout.EnumPopup(cameraAction.ActionType, GUILayout.Width(150));
            if (newActionType != cameraAction.ActionType)
            {
                cameraAction.ActionType = newActionType;
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCameraNameField(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Camera Name:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var newCameraName = EditorGUILayout.TextField(cameraAction.CameraName ?? "", GUILayout.Width(200));
            if (newCameraName != cameraAction.CameraName)
            {
                cameraAction.CameraName = newCameraName;
            }
            
            EditorGUILayout.EndHorizontal();
            
            if (string.IsNullOrEmpty(cameraAction.CameraName))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(120));
                EditorGUILayout.HelpBox("Enter the name of the Cinemachine Virtual Camera in the scene", MessageType.Info);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawTargetNameField(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Name:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var newTargetName = EditorGUILayout.TextField(cameraAction.TargetName ?? "", GUILayout.Width(200));
            if (newTargetName != cameraAction.TargetName)
            {
                cameraAction.TargetName = newTargetName;
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawDurationField(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Duration:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var newDuration = EditorGUILayout.FloatField(cameraAction.Duration, GUILayout.Width(100));
            if (newDuration != cameraAction.Duration)
            {
                cameraAction.Duration = Mathf.Max(0f, newDuration);
            }
            
            EditorGUILayout.LabelField("seconds", GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPriorityField(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Priority:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var newPriority = EditorGUILayout.IntField(cameraAction.Priority, GUILayout.Width(100));
            if (newPriority != cameraAction.Priority)
            {
                cameraAction.Priority = newPriority;
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFieldOfViewField(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Field of View:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var newFOV = EditorGUILayout.FloatField(cameraAction.FieldOfView, GUILayout.Width(100));
            if (newFOV != cameraAction.FieldOfView)
            {
                cameraAction.FieldOfView = Mathf.Clamp(newFOV, 1f, 179f);
            }
            
            EditorGUILayout.LabelField("degrees", GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPositionFields(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Position:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            EditorGUILayout.LabelField("X:", GUILayout.Width(15));
            var newX = EditorGUILayout.FloatField(cameraAction.PositionX, GUILayout.Width(60));
            
            EditorGUILayout.LabelField("Y:", GUILayout.Width(15));
            var newY = EditorGUILayout.FloatField(cameraAction.PositionY, GUILayout.Width(60));
            
            if (newX != cameraAction.PositionX) cameraAction.PositionX = newX;
            if (newY != cameraAction.PositionY) cameraAction.PositionY = newY;
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawIntensityField(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Intensity:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var newIntensity = EditorGUILayout.FloatField(cameraAction.Intensity, GUILayout.Width(100));
            if (newIntensity != cameraAction.Intensity)
            {
                cameraAction.Intensity = Mathf.Max(0f, newIntensity);
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawEaseTypeField(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Ease Type:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var easeOptions = new string[] { "Linear", "EaseIn", "EaseOut", "EaseInOut", "EaseOutBack", "EaseInBack" };
            var currentIndex = System.Array.IndexOf(easeOptions, cameraAction.EaseType);
            if (currentIndex == -1) currentIndex = 0;
            
            var newIndex = EditorGUILayout.Popup(currentIndex, easeOptions, GUILayout.Width(150));
            if (newIndex != currentIndex)
            {
                cameraAction.EaseType = easeOptions[newIndex];
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawWaitForCompletionField(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wait for Completion:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var newWaitForCompletion = EditorGUILayout.Toggle(cameraAction.WaitForCompletion, GUILayout.Width(50));
            if (newWaitForCompletion != cameraAction.WaitForCompletion)
            {
                cameraAction.WaitForCompletion = newWaitForCompletion;
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(120));
            EditorGUILayout.HelpBox("If enabled, story will wait for camera action to complete before continuing", MessageType.Info);
            EditorGUILayout.EndHorizontal();
        }
    }
}