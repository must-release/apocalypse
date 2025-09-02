using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.GamePlay
{
    public class EnemyAttackingState : EnemyStateBase
    {
        /****** Public Members ******/

        public override EnemyStateType StateType => EnemyStateType.Attacking;

        public override void OnEnter()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");

            base.OnEnter();

            _attackCTS = new CancellationTokenSource();
            ProgressAttackAsync().Forget();
        }

        public override void OnExit(EnemyStateType nextState)
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert(null != _attackCTS, $"Attack cancellation token should be set when exiting attacking state in {OwningCharacter.ActorName}.");

            base.OnExit(nextState);

            _attackCTS.Cancel();
            _attackCTS.Dispose();
            _attackCTS = null;
        }


        /****** Private Members ******/

        private CancellationTokenSource _attackCTS;

        private async UniTask ProgressAttackAsync()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert(null != _attackCTS, $"Attack cancellation token is not set in {OwningCharacter.ActorName}.");

            OpResult result = await OwningCharacter.AttackAsync(_attackCTS.Token);
            if (OpResult.Success == result)
            {
                StateController.ChangeState(EnemyStateType.Chasing);
            }
        }

        private void OnDestroy()
        {
            if (null != _attackCTS)
            {
                _attackCTS.Cancel();
                _attackCTS.Dispose();
                _attackCTS = null;
            }
        }
    }
}