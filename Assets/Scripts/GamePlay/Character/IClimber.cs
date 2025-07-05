using UnityEngine;

public interface IClimber
{
    IClimbable CurrentClimbableObject { get; set; }
    Collider2D ClimberCollider { get; }
}