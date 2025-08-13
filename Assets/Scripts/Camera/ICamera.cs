using UnityEngine;

public interface ICamera
{
    bool IsActive { get; }

    void ActivateCamera(Transform target);
    void DeactivateCamera();
}