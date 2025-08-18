using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using System.Threading;

namespace AD.Camera
{
    public class CameraManager : MonoBehaviour
    {
        /****** Public Members ******/

        public static CameraManager Instance { get; private set; }
        public ICamera CurrentCamera { get; private set; }
        public bool HasActiveCamera => null != CurrentCamera && CurrentCamera.IsActive;
        public List<ICamera> RegisteredCameras { get; private set; } = new List<ICamera>();

        public void RegisterCamera(ICamera camera)
        {
            Debug.Assert(null != camera, "Cannot register null camera.");

            if (false == RegisteredCameras.Contains(camera))
            {
                RegisteredCameras.Add(camera);
            }


            Logger.Write(LogCategory.GamePlay, $"Camera registered: {camera.GetType().Name}", LogLevel.Log, true);
        }

        public void RegisterCameras(ICamera[] cameras)
        {
            Debug.Assert(null != cameras, "Cannot register null camera array.");

            foreach (var camera in cameras)
            {
                if (null != camera)
                {
                    RegisterCamera(camera);
                }
            }

            Logger.Write(LogCategory.GamePlay, $"{cameras.Length} cameras registered", LogLevel.Log, true);
        }

        public void SetCurrentCamera(ICamera camera)
        {
            Debug.Assert(null != camera, "Cannot set null camera as current.");
            Debug.Assert(RegisteredCameras.Contains(camera), "Camera must be registered before setting as current.");

            CurrentCamera = camera;
            Logger.Write(LogCategory.GamePlay, $"Current camera set to: {camera.GetType().Name}", LogLevel.Log, true);
        }

        public void SetCurrentCamera<T>() where T : class, ICamera
        {
            var camera = GetCameraByType<T>();
            Debug.Assert(null != camera, $"No camera of type {typeof(T).Name} is registered.");

            SetCurrentCamera(camera);
        }

        public void SetCurrentCameraByName(string cameraName)
        {
            var camera = RegisteredCameras.FirstOrDefault(c => c.CameraName == cameraName);
            Debug.Assert(null != camera, $"No camera with name {cameraName} is registered.");

            SetCurrentCamera(camera);
        }

        public void ActivateCamera(Transform target = null)
        {
            Debug.Assert(null != CurrentCamera, "No camera is registered.");

            CurrentCamera.ActivateCamera(target);
        }

        public async UniTask ActivateCameraAsync(Transform target = null, CancellationToken cancellationToken = default)
        {
            Debug.Assert(null != CurrentCamera, "No camera is registered.");

            ActivateCamera(target);

            await UniTask.WaitWhile(() => _cinemachineBrain.IsBlending, cancellationToken: cancellationToken);
        }

        public void DeactivateCamera()
        {
            Debug.Assert(null != CurrentCamera, "No camera is registered.");

            CurrentCamera.DeactivateCamera();
            Logger.Write(LogCategory.GamePlay, "Camera deactivated", LogLevel.Log, true);
        }

        public void ClearCameras()
        {
            if (HasActiveCamera)
            {
                DeactivateCamera();
            }

            RegisteredCameras.Clear();
            CurrentCamera = null;

            Logger.Write(LogCategory.GamePlay, "All cameras cleared", LogLevel.Log, true);
        }

        public ICamera GetCameraByType<T>() where T : class, ICamera
        {
            return RegisteredCameras.OfType<T>().FirstOrDefault();
        }


        /****** Private Members ******/

        [SerializeField] private CinemachineBrain _cinemachineBrain;

        private void Awake()
        {
            if (null == Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnValidate()
        {
            Debug.Assert(null != _cinemachineBrain, "CinemachineBrain component is required on CameraManager.");
        }
    }
}
