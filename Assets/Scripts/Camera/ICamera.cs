using UnityEngine;

namespace AD.Camera
{
    public interface ICamera
    {
        bool IsActive { get; }

        void ActivateCamera(Transform target);
        void DeactivateCamera();
    }
}