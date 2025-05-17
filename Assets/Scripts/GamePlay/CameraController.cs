using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    private Transform mainCamera;
    private Transform attachedTarget;
    private float followingDelay;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            mainCamera = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (attachedTarget)
        {
            Vector3 targetPosition = new Vector3(attachedTarget.position.x, attachedTarget.position.y, mainCamera.position.z);
            mainCamera.position = Vector3.SmoothDamp(mainCamera.position, targetPosition, ref velocity, followingDelay);
        }
    }

    public void AttachCamera(Transform target, float delay)
    {
        attachedTarget = target;
        followingDelay = delay;
    }
}
