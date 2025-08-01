using UnityEngine;
using UnityEngine.Assertions;

public class StandingEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public string CharacterID { get; private set; }
    public ExpressionType Expression { get; private set; }
    public StoryCharacterStanding.AnimationType Animation { get; private set; }
    public bool IsBlockingAnimation { get; private set; }
    public float AnimationSpeed { get; private set; }
    public StoryCharacterStanding.TargetPositionType TargetPosition { get; private set; }

    public void Initialize(StoryCharacterStanding standingData)
    {
        Debug.Assert(false == IsInitialized, "Duplicate initialization of GameEventInfo is not allowed.");
        Debug.Assert(null != standingData, "StandingData cannot be null.");
        Debug.Assert(false == string.IsNullOrEmpty(standingData.Name), "CharacterID cannot be null or empty.");

        EventType           = GameEventType.Standing;
        CharacterID         = standingData.Name;
        Expression          = standingData.Expression;
        Animation           = standingData.Animation;
        IsBlockingAnimation = standingData.IsBlockingAnimation;
        AnimationSpeed      = standingData.AnimationSpeed;
        TargetPosition      = standingData.TargetPosition;
        IsInitialized       = true;
    }

    public override GameEventInfo Clone()
    {
        var clone = Instantiate(this);
        clone.EventType             = this.EventType;
        clone.CharacterID           = this.CharacterID;
        clone.Expression            = this.Expression;
        clone.Animation             = this.Animation;
        clone.IsBlockingAnimation   = this.IsBlockingAnimation;
        clone.AnimationSpeed        = this.AnimationSpeed;
        clone.TargetPosition        = this.TargetPosition;
        clone.IsInitialized         = this.IsInitialized;
        clone.IsRuntimeInstance     = true;

        return clone;
    }


    /****** protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.Standing;
    }

    protected override void OnValidate()
    {
        Debug.Assert(!string.IsNullOrEmpty(CharacterID), $"[{name}] CharacterID must be set.");
        Debug.Assert(AnimationSpeed > 0f, $"[{name}] AnimationSpeed must be positive. Current value: {AnimationSpeed}");

        if (!string.IsNullOrEmpty(CharacterID) && AnimationSpeed > 0f)
            IsInitialized = true;
    }
}