using UnityEngine;

public interface ICharacter 
{
    bool IsPlayer { get; }

    void ControlCharacter(ControlInfo controlInfo);
    void OnDamaged(DamageInfo damageInfo);
}
