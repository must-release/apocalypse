using UnityEngine;

public interface ICharacterInfo
{
    FacingDirection     CurrentFacingDirection      { get; }
    ControlInfo         CurrentControlInfo          { get; }
    GameObject          StandingGround              { get; }
    DamageInfo          RecentDamagedInfo           { get; }
    Vector2             CurrentVelocity             { get; }
    Vector3             CurrentPosition             { get; }

    float       CharacterHeight     { get; }
    float       MovingSpeed         { get; }
    float       JumpingSpeed        { get; }
    float       Gravity             { get; }
    int         MaxHitPoint         { get; }
    int         CurrentHitPoint     { get; }
}
