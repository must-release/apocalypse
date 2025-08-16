using UnityEngine;
using Unity.Cinemachine;
using Cysharp.Threading.Tasks;

namespace AD.Camera
{
    public class FollowCamera : MonoBehaviour, IGamePlayCamera
    {
        /****** Public Members ******/

        public CinemachineCamera VirtualCamera { get; private set; }
        public bool IsActive => VirtualCamera.Priority > 0;
        public string CameraName => gameObject.name;

        public void Initialize(BoxCollider2D stageBoundary)
        {
            Debug.Assert(null != stageBoundary, "StageBoundary cannot be null when initializing FollowCamera.");

            SetupVirtualCamera();
            SetupConfiner(stageBoundary);

            transform.position = new Vector3(0, 0, -10);
        }

        public void ActivateCamera(Transform playerTransform)
        {
            Debug.Assert(null != VirtualCamera, "VirtualCamera is not initialized.");
            Debug.Assert(null != playerTransform, "Player transform cannot be null in follow camera.");

            VirtualCamera.Follow = playerTransform;
            VirtualCamera.LookAt = playerTransform;
            VirtualCamera.Priority = 10;
        }

        public void DeactivateCamera()
        {
            Debug.Assert(null != VirtualCamera, "VirtualCamera is not initialized.");

            VirtualCamera.Priority = 0;
            VirtualCamera.Follow = null;
            VirtualCamera.LookAt = null;
        }

        public void SetCameraTarget(Transform target)
        {
            Debug.Assert(null != VirtualCamera, "VirtualCamera is not initialized.");
            Debug.Assert(null != target, "Target transform cannot be null.");

            VirtualCamera.Follow = target;
            VirtualCamera.LookAt = target;
        }


        /****** Private Members ******/

        private void SetupVirtualCamera()
        {
            VirtualCamera = GetComponent<CinemachineCamera>();
            if (null == VirtualCamera)
            {
                VirtualCamera = gameObject.AddComponent<CinemachineCamera>();
            }

            VirtualCamera.Priority = 0;
            VirtualCamera.Lens.FieldOfView = 60f;

            var positionComposer = GetComponent<CinemachinePositionComposer>();
            if (null == positionComposer)
            {
                positionComposer = VirtualCamera.gameObject.AddComponent<CinemachinePositionComposer>();
            }
            positionComposer.Damping = new Vector3(0.5f, 0.5f, 0.5f);
        }

        private void SetupConfiner(BoxCollider2D stageBoundary)
        {
            Debug.Assert(null != VirtualCamera, "VirtualCamera must be initialized before setting up confiner.");
            Debug.Assert(null != stageBoundary, "StageBoundary cannot be null.");

            CinemachineConfiner2D confiner = VirtualCamera.GetComponent<CinemachineConfiner2D>();
            if (null == confiner)
            {
                confiner = VirtualCamera.gameObject.AddComponent<CinemachineConfiner2D>();
            }

            confiner.BoundingShape2D = stageBoundary;
            confiner.Damping = 0.5f;
        }
    }
}