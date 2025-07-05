using UnityEngine;

public class ControlInfo : IReadOnlyControlInfo
{
    public HorizontalDirection  HorizontalInput { get; set; }
    public VerticalDirection    VerticalInput   { get; set; }
    public Vector3              AimingPosition  { get; set; }

    public bool IsJumpStarted   { get; set; }
    public bool IsJumping       { get; set; }
    public bool IsAttacking     { get; set; }
    public bool IsTagging       { get; set; }
}