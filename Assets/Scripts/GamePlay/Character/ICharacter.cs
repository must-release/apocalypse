using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace AD.GamePlay
{
    public interface ICharacter : IActor
    {
        CharacterStats          Stats       { get; }
        new CharacterMovement   Movement    { get; }
        
        bool IsPlayer { get; }

        event Action OnCharacterDeath;
        event Action OnCharacterDamaged;
        
        void ControlCharacter(IReadOnlyControlInfo controlInfo);
        void ApplyDamage(DamageInfo damageInfo);
    }
}