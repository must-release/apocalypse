using UnityEngine;
using UnityEngine.Assertions;

public class StandingEventInfo : GameEventInfo
{
    public string CharacterID { get; private set; }
    public string Expression { get; private set; }
    public StoryCharacterStanding.AnimationType Animation { get; private set; }
    public bool IsBlockingAnimation { get; private set; }
    public float AnimationSpeed { get; private set; }
    public StoryCharacterStanding.TargetPositionType TargetPosition { get; private set; }

    public void Initialize(StoryCharacterStanding standingData)
    {
        Debug.Assert(null != standingData, "StandingData cannot be null.");
        Debug.Assert(false == string.IsNullOrEmpty(standingData.Name), "CharacterID cannot be null or empty.");

        EventType = GameEventType.Standing;
        CharacterID = standingData.Name;
        Expression = standingData.Expression;
        Animation = standingData.Animation;
        IsBlockingAnimation = standingData.IsBlockingAnimation;
        AnimationSpeed = standingData.AnimationSpeed;
        TargetPosition = standingData.TargetPosition;
        IsInitialized = true;
    }

    public override GameEventInfo Clone()
    {
        StandingEventInfo clone = CreateInstance<StandingEventInfo>();
        clone.EventType = this.EventType;
        clone.CharacterID = this.CharacterID;
        clone.Expression = this.Expression;
        clone.Animation = this.Animation;
        clone.IsBlockingAnimation = this.IsBlockingAnimation;
        clone.AnimationSpeed = this.AnimationSpeed;
        clone.TargetPosition = this.TargetPosition;
        clone.IsInitialized = this.IsInitialized;
        clone.IsRuntimeInstance = true;
        return clone;
    }

    protected override void OnEnable()
    {
        // No specific logic needed for now
    }

    protected override void OnValidate()
    {
        // No specific logic needed for now
    }
}