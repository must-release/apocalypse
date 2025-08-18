using UnityEngine;
using UnityEditor;
using AD.Story;
using AD.Camera;
using AD.GamePlay;
using Unity.Cinemachine;
using System.Linq;

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
        private string[] _gameplayCameraNames;
        private string[] _actorNames;

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
                case CameraActionType.SwitchToCamera:
                    DrawCameraNameField(cameraAction);
                    DrawIsTargetPlayerField(cameraAction);
                    DrawTargetNameField(cameraAction);
                    break;

                case CameraActionType.FollowTarget:
                    DrawIsTargetPlayerField(cameraAction);
                    DrawTargetNameField(cameraAction);
                    break;

                case CameraActionType.SetPriority:
                    DrawCameraNameField(cameraAction);
                    DrawPriorityField(cameraAction);
                    break;

                case CameraActionType.Zoom:
                    DrawFieldOfViewField(cameraAction);
                    DrawDurationField(cameraAction);
                    DrawEaseTypeField(cameraAction);
                    break;

                case CameraActionType.MoveTo:
                    DrawPositionFields(cameraAction);
                    DrawDurationField(cameraAction);
                    DrawEaseTypeField(cameraAction);
                    break;

                case CameraActionType.Shake:
                    DrawIntensityField(cameraAction);
                    DrawDurationField(cameraAction);
                    break;
            }

        }

        private void DrawActionTypeField(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Action Type:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var newActionType = (CameraActionType)EditorGUILayout.EnumPopup(cameraAction.ActionType, GUILayout.Width(150));
            if (newActionType != cameraAction.ActionType)
            {
                cameraAction.ActionType = newActionType;
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCameraNameField(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Camera:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            if (null == _gameplayCameraNames || 0 == _gameplayCameraNames.Length)
            {
                EditorGUILayout.LabelField("No GamePlay Cameras found", GUILayout.Width(200));
            }
            else
            {
                var currentIndex = System.Array.IndexOf(_gameplayCameraNames, cameraAction.CameraName);
                if (currentIndex == -1) 
                {
                    currentIndex = 0;
                    // 초기값이 설정되지 않았다면 첫 번째 카메라로 자동 설정
                    if (string.IsNullOrEmpty(cameraAction.CameraName) && _gameplayCameraNames.Length > 0)
                    {
                        cameraAction.CameraName = _gameplayCameraNames[0];
                    }
                }
                
                var newIndex = EditorGUILayout.Popup(currentIndex, _gameplayCameraNames, GUILayout.Width(200));
                if (newIndex != currentIndex && newIndex >= 0 && newIndex < _gameplayCameraNames.Length)
                {
                    cameraAction.CameraName = _gameplayCameraNames[newIndex];
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            if (null == _stagePrefab)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(120));
                EditorGUILayout.HelpBox("Select a Stage Prefab first to see available GamePlay Cameras", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }
            else if (null == _gameplayCameraNames || 0 == _gameplayCameraNames.Length)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(120));
                EditorGUILayout.HelpBox("No GamePlay Cameras found in the selected Stage Prefab", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawTargetNameField(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Actor:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            // IsTargetPlayer가 활성화되면 UI 비활성화
            bool wasEnabled = GUI.enabled;
            if (cameraAction.IsTargetPlayer)
            {
                GUI.enabled = false;
                // IsTargetPlayer가 true일 때 TargetName을 null로 설정
                cameraAction.TargetName = null;
            }
            
            if (null == _actorNames || 0 == _actorNames.Length)
            {
                // None만 표시
                var noneOptions = new string[] { "None" };
                var selectedIndex = EditorGUILayout.Popup(0, noneOptions, GUILayout.Width(200));
                if (false == cameraAction.IsTargetPlayer)
                {
                    cameraAction.TargetName = null;
                }
            }
            else
            {
                // None 옵션을 첫 번째에 추가
                var optionsWithNone = new string[_actorNames.Length + 1];
                optionsWithNone[0] = "None";
                System.Array.Copy(_actorNames, 0, optionsWithNone, 1, _actorNames.Length);
                
                int currentIndex = 0; // 기본값은 None
                if (false == cameraAction.IsTargetPlayer && false == string.IsNullOrEmpty(cameraAction.TargetName))
                {
                    var actorIndex = System.Array.IndexOf(_actorNames, cameraAction.TargetName);
                    if (actorIndex >= 0)
                    {
                        currentIndex = actorIndex + 1; // None이 0번이므로 +1
                    }
                }
                
                var newIndex = EditorGUILayout.Popup(currentIndex, optionsWithNone, GUILayout.Width(200));
                if (false == cameraAction.IsTargetPlayer && newIndex != currentIndex)
                {
                    if (0 == newIndex)
                    {
                        // None 선택
                        cameraAction.TargetName = null;
                    }
                    else if (newIndex >= 1 && newIndex <= _actorNames.Length)
                    {
                        // 액터 선택 (인덱스 조정)
                        cameraAction.TargetName = _actorNames[newIndex - 1];
                    }
                }
            }
            
            // GUI 활성화 상태 복원
            GUI.enabled = wasEnabled;
            
            EditorGUILayout.EndHorizontal();
            
            if (cameraAction.IsTargetPlayer)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(120));
                EditorGUILayout.HelpBox("Target Actor is disabled when Target Player is enabled", MessageType.Info);
                EditorGUILayout.EndHorizontal();
            }
            else if (null == _stagePrefab)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(120));
                EditorGUILayout.HelpBox("Select a Stage Prefab first to see available Actors", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }
            else if (null == _actorNames || 0 == _actorNames.Length)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(120));
                EditorGUILayout.HelpBox("No Actors found in the selected Stage Prefab", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawIsTargetPlayerField(StoryCameraAction cameraAction)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Follow Player:", EditorStyles.boldLabel, GUILayout.Width(120));
            
            var newIsTargetPlayer = EditorGUILayout.Toggle(cameraAction.IsTargetPlayer, GUILayout.Width(50));
            if (newIsTargetPlayer != cameraAction.IsTargetPlayer)
            {
                cameraAction.IsTargetPlayer = newIsTargetPlayer;
                // IsTargetPlayer가 활성화되면 TargetName을 null로 설정
                if (cameraAction.IsTargetPlayer)
                {
                    cameraAction.TargetName = null;
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(120));
            EditorGUILayout.HelpBox("If enabled, camera will target the player instead of a specific actor", MessageType.Info);
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
                // Stage Prefab이 변경되면 CameraName과 TargetName 초기화
                cameraAction.CameraName = "";
                cameraAction.TargetName = null; // None 상태로 초기화
                UpdateVirtualCameraList();
                UpdateActorList();
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
                _gameplayCameraNames = null;
                return;
            }

            var gameplayCameras = _stagePrefab.GetComponentsInChildren<MonoBehaviour>(true)
                .Where(component => component is AD.Camera.IGamePlayCamera)
                .Cast<AD.Camera.IGamePlayCamera>()
                .ToArray();
                
            _gameplayCameraNames = new string[gameplayCameras.Length];
            
            for (int i = 0; i < gameplayCameras.Length; i++)
            {
                _gameplayCameraNames[i] = ((MonoBehaviour)gameplayCameras[i]).name;
            }
        }

        private void UpdateActorList()
        {
            if (null == _stagePrefab)
            {
                _actorNames = null;
                return;
            }

            var actors = _stagePrefab.GetComponentsInChildren<MonoBehaviour>(true)
                .Where(component => component is IActor)
                .Cast<IActor>()
                .ToArray();
                
            _actorNames = new string[actors.Length];
            
            for (int i = 0; i < actors.Length; i++)
            {
                _actorNames[i] = actors[i].ActorName;
            }
        }

    }
}