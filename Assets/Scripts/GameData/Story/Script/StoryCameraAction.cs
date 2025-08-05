using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class StoryCameraAction : StoryEntry
{
    public enum CameraActionType
    {
        SwitchToCamera,       
        FollowTarget,         
        SetPriority,           
        Zoom,                  
        MoveTo,                 
        Shake,                 
        ResetToDefault
    }

    public StoryCameraAction() { }

    [XmlAttribute("ActionType")]
    public CameraActionType ActionType;

    [XmlAttribute("CameraName")]
    public string CameraName;

    [XmlAttribute("TargetName")]
    public string TargetName;

    [XmlAttribute("Duration")]
    public float Duration = 1.0f;

    [XmlAttribute("Priority")]
    public int Priority = 10;

    [XmlAttribute("FieldOfView")]
    public float FieldOfView = 60f;

    [XmlAttribute("PositionX")]
    public float PositionX;

    [XmlAttribute("PositionY")]
    public float PositionY;

    [XmlAttribute("PositionZ")]
    public float PositionZ;

    [XmlAttribute("Intensity")]
    public float Intensity = 1.0f;

    [XmlAttribute("EaseType")]
    public string EaseType = "Linear";

    [XmlAttribute("WaitForCompletion")]
    public bool WaitForCompletion = true;
}