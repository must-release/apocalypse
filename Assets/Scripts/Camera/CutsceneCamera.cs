using UnityEngine;

namespace AD.Camera
{
    public class CutsceneCamera : MonoBehaviour, IGamePlayCamera
    {
        /****** Public Members ******/

        public bool IsActive => gameObject.activeSelf;
        public string CameraName => gameObject.name;

        public void Initialize(BoxCollider2D stageBoundary)
        {
            Debug.Assert(null != stageBoundary, "StageBoundary cannot be null when initializing CutsceneCamera.");
            transform.position = new Vector3(0, 0, -10);
        }

        public void SetCameraTarget(Transform target)
        {
            Debug.Assert(null != target, "Target transform cannot be null for CutsceneCamera.");
            transform.position = target.position;
        }

        public void ActivateCamera(Transform target)
        {
            Debug.Assert(null != target, "Target transform cannot be null for CutsceneCamera.");
            gameObject.SetActive(true);
            transform.position = target.position;
        }

        public void DeactivateCamera()
        {
            gameObject.SetActive(false);
            Logger.Write(LogCategory.GameScene, "CutsceneCamera deactivated", LogLevel.Log, true);
        }
    }
}