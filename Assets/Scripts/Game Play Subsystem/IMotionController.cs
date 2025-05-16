using UnityEngine;

/// <summary>
/// Interface for motion control methods.
/// This interface defines methods for controlling the motion of a object.
/// </summary>
public interface IMotionController
{
    /// <summary>
    /// Sets the velocity of the Rigidbody directly.
    /// </summary>
    void SetVelocity(Vector2 velocity);

    /// <summary>
    /// Adds a force to the Rigidbody.
    /// </summary>
    void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force);

    /// <summary>
    /// Clears the current velocity of the Rigidbody.
    /// </summary>
    void ResetVelocity();

    /// <summary>
    /// Sets the angular (rotation) velocity.
    /// </summary>
    void SetAngularVelocity(float angularVelocity);

    /// <summary>
    /// Sets the gravity scale of the Rigidbody.
    /// </summary>
    void SetGravityScale(float scale);

    /// <summary>
    /// Instantly sets the object's position, bypassing physics simulation.
    /// </summary>
    void TeleportTo(Vector2 position);

    /// <summary>
    /// Sets the facing direction of the object.
    /// </summary>
    void SetFacingDirection(FacingDirection direction);
}