using System.Xml.Serialization;

[System.Serializable]
public class StoryCharacterCG : StoryEntry
{
    public enum AnimationType
    {
        None = 0,
        Appear,
        Disappear,
        Move
    }

    public enum TargetPositionType
    {
        Center = 0,
        Left,
        Right
    }

    public enum FacialExpressionType
    {
        Default = 0,
        Smile,
        Cry,
        Rage,
        FacialExpressionTypeCount // Keep this last for count
    }

    public StoryCharacterCG() { }

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("Expression")]
    public FacialExpressionType Expression;

    [XmlAttribute("Animation")]
    public AnimationType Animation;

    [XmlAttribute("IsBlockingAnimation")]
    public bool IsBlockingAnimation = true; // If true, next entry should wait for this animation to be over

    [XmlAttribute("AnimationSpeed")]
    public float AnimationSpeed = 1.0f;

    [XmlAttribute("TargetPosition")]
    public TargetPositionType TargetPosition;
}