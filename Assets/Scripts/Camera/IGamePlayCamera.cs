using UnityEngine;

namespace AD.Camera
{
    public interface IGamePlayCamera : ICamera
    {
        void Initialize(BoxCollider2D stageBoundary);
        void SetCameraTarget(Transform target);
    }
}
