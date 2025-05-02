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
    /// <param name="velocity">The new velocity vector.</param>
    void SetVelocity(Vector2 velocity);

    /// <summary>
    /// Adds a force to the Rigidbody.
    /// </summary>
    /// <param name="force">The force vector to apply.</param>
    /// <param name="mode">The force mode (default is Force).</param>
    void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force);

    /// <summary>
    /// Clears the current velocity of the Rigidbody.
    /// </summary>
    void ResetVelocity();

    /// <summary>
    /// Sets the angular (rotation) velocity.
    /// </summary>
    /// <param name="angularVelocity">The angular velocity value.</param>
    void SetAngularVelocity(float angularVelocity);

    /// <summary>
    /// Sets the gravity scale of the Rigidbody.
    /// </summary>
    /// <param name="scale">The new gravity scale value.</param>
    void SetGravityScale(float scale);

    /// <summary>
    /// Instantly sets the object's position, bypassing physics simulation.
    /// </summary>
    /// <param name="position">The position to teleport.</param>
    void TeleportTo(Vector2 position);

    /// <summary>
    /// Sets the facing direction of the object.
    /// </summary>
    /// <param name="direction">The direction to face (1 for right, -1 for left).</param>
    void SetFacingDirection(FacingDirection direction);
}