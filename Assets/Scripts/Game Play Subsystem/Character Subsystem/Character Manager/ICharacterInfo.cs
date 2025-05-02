using UnityEngine;

public interface ICharacterInfo
{
    FacingDirection     FacingDirection     { get; }
    ControlInfo         CurrentControlInfo  { get; }
    GameObject          StandingGround      { get; }
    DamageInfo          RecentDamagedInfo   { get; }
    Vector2             CurrentVelocity     { get; }
    Vector2             CurrentPosition     { get; }

    float       CharacterHeight     { get; }
    float       MovingSpeed         { get; }
    float       JumpingSpeed        { get; }
    float       Gravity             { get; }
    int         MaxHitPoint         { get; }
    int         CurrentHitPoint     { get; }
}
