using UnityEngine;
using Cysharp.Threading.Tasks;

namespace AD.Camera
{
    public interface ICamera
    {
        bool IsActive { get; }
        string CameraName { get; }

        void ActivateCamera(Transform target = null);
        void DeactivateCamera();
    }
}