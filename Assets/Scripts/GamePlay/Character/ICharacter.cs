using UnityEngine;

public interface ICharacter : AD.GamePlay.IActor
{
    bool IsPlayer { get; }

    void ControlCharacter(IReadOnlyControlInfo controlInfo);
    void OnDamaged(DamageInfo damageInfo);
}
