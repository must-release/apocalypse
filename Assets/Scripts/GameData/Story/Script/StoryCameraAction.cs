using System.Xml.Serialization;
using UnityEngine;
using AD.Camera;

namespace AD.Story
{
    [System.Serializable]
    public class StoryCameraAction : StoryEntry
    {
        public StoryCameraAction() 
        {
            Type = EntryType.CameraAction;
        }

        [XmlAttribute("ActionType")]
        public CameraActionType ActionType;

        [XmlAttribute("TargetCamera")]
        public string TargetCamera;

        [XmlAttribute("IsTargetPlayer")]
        public bool IsTargetPlayer;

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
}