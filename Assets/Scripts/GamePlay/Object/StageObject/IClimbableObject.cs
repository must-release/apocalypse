using UnityEngine;

public interface IClimbableObject
{
    Vector3 GetClimbReferencePoint();  // Climb up if a climber is under the reference point. Otherwise climb down.
    void OnClimbStart(IObjectClimber climber);
    void OnClimbEnd(IObjectClimber climber);
    bool CanClimbFurther(Vector3 climberPosition, VerticalDirection direction);
}
