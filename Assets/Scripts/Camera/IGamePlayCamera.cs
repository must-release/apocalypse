using UnityEngine;

interface IGamePlayCamera : ICamera
{
    void Initialize(BoxCollider2D stageBoundary);
    void SetCameraTarget(Transform target);
}