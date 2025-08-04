using System.Xml.Serialization;

[System.Serializable]
public class StoryCharacterStanding : StoryEntry
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

    public StoryCharacterStanding() { }

    [XmlAttribute("Name")]
    public string Name;

    [XmlAttribute("Expression")]
    public string Expression;

    [XmlAttribute("Animation")]
    public AnimationType Animation;

    [XmlAttribute("IsBlockingAnimation")]
    public bool IsBlockingAnimation = true; // If true, next entry should wait for this animation to be over

    [XmlAttribute("AnimationSpeed")]
    public float AnimationSpeed;

    [XmlAttribute("TargetPosition")]
    public TargetPositionType TargetPosition;
}