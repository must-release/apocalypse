using UnityEngine;

public interface IClimbableObject
{
    Vector3 GetClimbReferencePoint();
    void OnClimbStart(IObjectClimber climber);
    void OnClimbEnd(IObjectClimber climber);
    bool CanClimbFurther(Vector3 climberPosition, VerticalDirection direction);
}
