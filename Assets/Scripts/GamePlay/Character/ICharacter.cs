
namespace AD.GamePlay
{
    public interface ICharacter : IActor
    {
        CharacterStats          Stats       { get; }
        CharacterMovement       Movement    { get; }
        
        bool IsPlayer { get; }
        
        void ControlCharacter(IReadOnlyControlInfo controlInfo);
        void OnDamaged(DamageInfo damageInfo);
    }
}