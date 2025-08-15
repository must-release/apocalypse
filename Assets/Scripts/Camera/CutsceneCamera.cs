using UnityEngine;
using Unity.Cinemachine;

namespace AD.Camera
{

    [RequireComponent(typeof(CinemachineCamera))]
    [RequireComponent(typeof(CinemachinePositionComposer))]
    [RequireComponent(typeof(CinemachineConfiner2D))]
    public class CutsceneCamera : MonoBehaviour, IGamePlayCamera
    {
        /****** Public Members ******/

        public bool IsActive => gameObject.activeSelf;
        public string CameraName => gameObject.name;

        public void Initialize(BoxCollider2D stageBoundary)
        {
            Debug.Assert(null != stageBoundary, "StageBoundary cannot be null when initializing FollowCamera.");

            SetupVirtualCamera();
            SetupConfiner(stageBoundary);
        }

        public void ActivateCamera(Transform target = null)
        {
            Debug.Assert(null != _virtualCamera, $"VirtualCamera is not initialized in{CameraName}.");

            _virtualCamera.Follow = target;
            _virtualCamera.LookAt = target;
            _virtualCamera.Priority = 10;
        }

        public void DeactivateCamera()
        {
            Debug.Assert(null != _virtualCamera, $"VirtualCamera is not initialized in {CameraName}.");

            _virtualCamera.Priority = 0;
            _virtualCamera.Follow = null;
            _virtualCamera.LookAt = null;
        }

        public void SetCameraTarget(Transform target)
        {
            Debug.Assert(null != target, "Target transform cannot be null for CutsceneCamera.");

            _virtualCamera.Follow = target;
            _virtualCamera.LookAt = target;
        }


        /****** Private Members ******/

        private CinemachineCamera _virtualCamera;

        private void SetupVirtualCamera()
        {
            _virtualCamera              = GetComponent<CinemachineCamera>();
            var positionComposer        = GetComponent<CinemachinePositionComposer>();
            positionComposer.Damping    = new Vector3(0.5f, 0.5f, 0.5f);
        }

        private void SetupConfiner(BoxCollider2D stageBoundary)
        {
            Debug.Assert(null != _virtualCamera, "VirtualCamera must be initialized before setting up confiner.");
            Debug.Assert(null != stageBoundary, "StageBoundary cannot be null.");

            CinemachineConfiner2D confiner = _virtualCamera.GetComponent<CinemachineConfiner2D>();
            confiner.BoundingShape2D = stageBoundary;
            confiner.Damping = 0.5f;
        }
    }
}