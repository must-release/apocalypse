using UnityEngine;

public interface ICharacter 
{
    bool IsPlayer { get; }

    void ControlCharacter(IReadOnlyControlInfo controlInfo);
    void OnDamaged(DamageInfo damageInfo);
}
