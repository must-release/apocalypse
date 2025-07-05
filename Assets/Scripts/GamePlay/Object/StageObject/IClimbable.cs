using UnityEngine;

public interface IClimbable
{
    Vector3 GetClimbReferencePoint();
    void OnClimbStart(IClimber climber);
    void OnClimbEnd(IClimber climber);
    bool CanClimbFurther(Vector3 climberPosition, VerticalDirection direction);
}
