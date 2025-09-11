using UnityEngine;

public interface IObjectClimber
{
    IClimbableObject CurrentClimbableObject { get; set; }
    Collider2D ClimberCollider { get; }
}