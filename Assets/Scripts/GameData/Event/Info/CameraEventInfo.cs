using UnityEngine;
using System;
using AD.Camera;

[Serializable]
[CreateAssetMenu(fileName = "NewCameraEventInfo", menuName = "EventInfo/CameraEvent", order = 0)]
public class CameraEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public CameraActionType ActionType  => _actionType;
    public string           TargetName  => _targetName;

    public void Initialize(CameraActionType actionType, string targetName = null)
    {
        _actionType = actionType;
        _targetName = targetName;
        IsInitialized = true;
        IsRuntimeInstance = true;
    }

    public override GameEventInfo Clone()
    {
        var clone = Instantiate(this);
        clone.IsRuntimeInstance = true;

        return clone;
    }


    /****** Protected Members ******/


    protected override void OnEnable()
    {
        EventType = GameEventType.Camera;
    }

    protected override void OnValidate()
    {

    }


    /****** Private Members ******/

    [SerializeField] private CameraActionType   _actionType = CameraActionType.SwitchToCamera;
    [SerializeField] private string             _targetName = "";
}