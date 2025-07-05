using UnityEngine;

public interface IReadOnlyControlInfo
{
    HorizontalDirection  HorizontalInput { get; }
    VerticalDirection    VerticalInput   { get; }
    Vector3              AimingPosition  { get; }

    bool IsJumpStarted   { get; }
    bool IsJumping       { get; }
    bool IsAttacking     { get; }
    bool IsTagging       { get; }
}
