using UnityEngine;
using UnityEditor;
using Unity.Cinemachine;

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

        private GameObject _stagePrefab;
        private string[] _virtualCameraNames;

        private void DrawCameraActionEditor(StoryCameraAction cameraAction)
        {
            EditorGUILayout.LabelField("Camera Action Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawStagePrefabField(cameraAction);
            EditorGUILayout.Space();

            DrawActionTypeField(cameraAction);
            EditorGUILayout.Space();

            // Draw fields based on action type
            switch (cameraAction.ActionType)
            {
                case StoryCameraAction.CameraActionType.SwitchToCamera:
                    DrawTargetCameraField(cameraAction);
                    break;

                case StoryCameraAction.CameraActionType.FollowTarget:
                    DrawTargetNameField(cameraAction);
                    break;

                case StoryCameraAction.CameraActionType.SetPriority:
                    DrawTargetCameraField(cameraAction);
                    DrawPriorityField(cameraAction);
                    break;

                case StoryCameraAction.CameraActionType.Zoom:
                    DrawFieldOfViewField(cameraAction);
                    DrawDurationField(cameraAction);
                    DrawEaseTypeField(cameraAction);
                    break;

                case StoryCameraAction.CameraActionType.MoveTo:
                    DrawPositionFields(cameraAction);
                    DrawDurationField(cameraAction);
                    DrawEaseTypeField(cameraAction);
                    break;

                case StoryCameraAction.CameraActionType.Shake:
                    DrawIntensityField(cameraAction);
                    DrawDurationField(cameraAction);
                    break;

                case StoryCameraAction.CameraActionType.ResetToDefault:
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

        private void DrawTargetCameraField(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Camera:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            if (null == _virtualCameraNames || 0 == _virtualCameraNames.Length)
            {
                EditorGUILayout.LabelField("No Virtual Cameras found", GUILayout.Width(200));
            }
            else
            {
                var currentIndex = System.Array.IndexOf(_virtualCameraNames, cameraAction.TargetCamera);
                if (currentIndex == -1) 
                {
                    currentIndex = 0;
                    // 초기값이 설정되지 않았다면 첫 번째 카메라로 자동 설정
                    if (string.IsNullOrEmpty(cameraAction.TargetCamera) && _virtualCameraNames.Length > 0)
                    {
                        cameraAction.TargetCamera = _virtualCameraNames[0];
                    }
                }
                
                var newIndex = EditorGUILayout.Popup(currentIndex, _virtualCameraNames, GUILayout.Width(200));
                if (newIndex != currentIndex && newIndex >= 0 && newIndex < _virtualCameraNames.Length)
                {
                    cameraAction.TargetCamera = _virtualCameraNames[newIndex];
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            if (null == _stagePrefab)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(120));
                EditorGUILayout.HelpBox("Select a Stage Prefab first to see available Virtual Cameras", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }
            else if (null == _virtualCameraNames || 0 == _virtualCameraNames.Length)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(120));
                EditorGUILayout.HelpBox("No Virtual Cameras found in the selected Stage Prefab", MessageType.Warning);
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

        private void DrawStagePrefabField(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Stage Prefab:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var newStagePrefab = (GameObject)EditorGUILayout.ObjectField(_stagePrefab, typeof(GameObject), false, GUILayout.Width(200));
            if (newStagePrefab != _stagePrefab)
            {
                _stagePrefab = newStagePrefab;
                // Stage Prefab이 변경되면 TargetCamera 초기화
                cameraAction.TargetCamera = "";
                UpdateVirtualCameraList();
            }
            
            EditorGUILayout.EndHorizontal();
            
            if (null == _stagePrefab)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(120));
                EditorGUILayout.HelpBox("Select a stage prefab for camera context", MessageType.Info);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                var stageManager = _stagePrefab.GetComponent<StageManager>();
                if (null == stageManager)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(120));
                    EditorGUILayout.HelpBox("Selected prefab must contain a StageManager component!", MessageType.Error);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void UpdateVirtualCameraList()
        {
            if (null == _stagePrefab)
            {
                _virtualCameraNames = null;
                return;
            }

            var virtualCameras = _stagePrefab.GetComponentsInChildren<CinemachineCamera>(true);
            _virtualCameraNames = new string[virtualCameras.Length];
            
            for (int i = 0; i < virtualCameras.Length; i++)
            {
                _virtualCameraNames[i] = virtualCameras[i].name;
            }
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