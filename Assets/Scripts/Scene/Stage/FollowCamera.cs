using UnityEngine;
using UnityEngine.Assertions;
using Unity.Cinemachine;

public class FollowCamera : MonoBehaviour
{
    /****** Public Members ******/
    
    public CinemachineCamera VirtualCamera { get; private set; }
    public bool IsActive => VirtualCamera.Priority > 0;
    
    public void Initialize(BoxCollider2D stageBoundary)
    {
        Assert.IsTrue(null != stageBoundary, "StageBoundary cannot be null when initializing FollowCamera.");
        
        SetupVirtualCamera();
        SetupConfiner(stageBoundary);
    }
    
    public void ActivateCamera(Transform playerTransform)
    {
        Assert.IsTrue(null != VirtualCamera, "VirtualCamera is not initialized.");
        Assert.IsTrue(null != playerTransform, "Player transform cannot be null.");
        
        VirtualCamera.Follow = playerTransform;
        VirtualCamera.LookAt = playerTransform;
        VirtualCamera.Priority = 10;
        
        Logger.Write(LogCategory.GameScene, $"FollowCamera activated for player at {playerTransform.position}", LogLevel.Log, true);
    }
    
    public void DeactivateCamera()
    {
        Assert.IsTrue(null != VirtualCamera, "VirtualCamera is not initialized.");
        
        VirtualCamera.Priority = 0;
        VirtualCamera.Follow = null;
        VirtualCamera.LookAt = null;
        
        Logger.Write(LogCategory.GameScene, "FollowCamera deactivated", LogLevel.Log, true);
    }
    
    public void SetCameraTarget(Transform target)
    {
        Assert.IsTrue(null != VirtualCamera, "VirtualCamera is not initialized.");
        Assert.IsTrue(null != target, "Target transform cannot be null.");
        
        VirtualCamera.Follow = target;
        VirtualCamera.LookAt = target;
    }
    
    
    /****** Private Members ******/

    private void SetupVirtualCamera()
    {
        VirtualCamera = gameObject.AddComponent<CinemachineCamera>();
        VirtualCamera.Priority = 0;
        VirtualCamera.Lens.FieldOfView = 60f;

        var positionComposer = VirtualCamera.gameObject.AddComponent<CinemachinePositionComposer>();
        positionComposer.Damping = new Vector3(0.5f, 0.5f, 0.5f);
    }
    
    private void SetupConfiner(BoxCollider2D stageBoundary)
    {
        Assert.IsTrue(null != VirtualCamera, "VirtualCamera must be initialized before setting up confiner.");
        Assert.IsTrue(null != stageBoundary, "StageBoundary cannot be null.");
        
        CinemachineConfiner2D confiner = VirtualCamera.GetComponent<CinemachineConfiner2D>();
        if (null == confiner)
        {
            confiner = VirtualCamera.gameObject.AddComponent<CinemachineConfiner2D>();
        }
        
        confiner.BoundingShape2D = stageBoundary;
        confiner.Damping = 0.5f;
    }
}