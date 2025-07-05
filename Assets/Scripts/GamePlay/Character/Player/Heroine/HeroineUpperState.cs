using UnityEngine;

public interface IHeroineUpperState
{
    void Move(HorizontalDirection horizontalInput);
    void LookUp(bool lookUp);
    public virtual void Jump() { /* Do not call base method in derived method */ }
    public override void Enable() { /* Do not call base method in derived method */ }
    public override void Disable() { /* Do not call base method in derived method */ }
    public virtual void Aim(Vector3 aim) { /* Do not call base method in derived method */ }
}
