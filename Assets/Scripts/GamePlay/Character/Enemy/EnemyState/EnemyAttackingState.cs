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

            ProgressAttackAsync().Forget();
        }


        /****** Private Members ******/

        private async UniTask ProgressAttackAsync()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");

            OpResult result = await EnemyCharacter.AttackAsync(this.GetCancellationTokenOnDestroy());
            if (result == OpResult.Success)
            {
                StateController.ChangeState(EnemyStateType.Chasing);
            }
        }
    }
}