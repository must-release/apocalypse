using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.GamePlay
{
    public class EnemyPatrollingState : EnemyStateBase
    {
        public override EnemyStateType StateType => EnemyStateType.Patrolling;

        public override void OnEnter()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");

            base.OnEnter();

            _patrolCTS = new CancellationTokenSource();
            ProgressPatrolAsync().Forget();
        }

        public override void OnUpdate()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");

            base.OnUpdate();

            if (EnemyPerception.HasDetectedPlayer)
            {
                StateController.ChangeState(EnemyStateType.Chasing);
            }
        }

        public override void OnExit(EnemyStateType nextState)
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert(null != _patrolCTS, $"Patrol cancellation token should be set when exiting patrolling state in {EnemyCharacter.ActorName}.");

            base.OnExit(nextState);

            _patrolCTS.Cancel();
            _patrolCTS.Dispose();
            _patrolCTS = null;
        }


        /****** Private Members ******/

        private CancellationTokenSource _patrolCTS;

        private async UniTask ProgressPatrolAsync()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert(null != _patrolCTS, $"Patrol cancellation token is not set in {EnemyCharacter.ActorName}.");

            OpResult result = await EnemyCharacter.PatrolAsync(_patrolCTS.Token);
            Debug.Assert(OpResult.Failed != result, $"Patrol failed in {EnemyCharacter.ActorName}.");
        }

        private void OnDestroy()
        {
            if (null != _patrolCTS)
            {
                _patrolCTS.Cancel();
                _patrolCTS.Dispose();
                _patrolCTS = null;
            }
        }
    }
}