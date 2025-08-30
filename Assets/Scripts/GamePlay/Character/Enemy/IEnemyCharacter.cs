using System.Threading;
using Cysharp.Threading.Tasks;

namespace AD.GamePlay
{
    public interface IEnemyCharacter : ICharacter
    {
        new EnemyCharacterStats Stats { get; }

        UniTask<OpResult> AttackAsync(CancellationToken cancellationToken);
        UniTask<OpResult> ChaseAsync(IActor chasingTarget, CancellationToken cancellationToken);
        UniTask<OpResult> PatrolAsync(CancellationToken cancellationToken);
        UniTask<OpResult> DieAsync(CancellationToken cancellationToken);
    }
}