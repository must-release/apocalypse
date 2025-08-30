using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.GamePlay
{
    public class EnemyChasingState : EnemyStateBase
    {
        public override EnemyStateType StateType => EnemyStateType.Chasing;

        public override void OnEnter()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");

            base.OnEnter();

            SetChasingTarget();
            Debug.Assert(null != _chasingTarget, $"Cannot find player character to chase in {OwningCharacter.ActorName}.");

            _chaseCTS = new CancellationTokenSource();
            ProgressChaseAsync().Forget();

            _forgettingTimePassed = 0;
        }

        public override void OnUpdate()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");

            base.OnUpdate();

            if (Perception.IsPlayerInAttackRange)
            {
                StateController.ChangeState(EnemyStateType.Attacking);
                return;
            }

            if (Perception.HasDetectedPlayer)
            {
                _forgettingTimePassed = 0;
            }
            else
            {
                _forgettingTimePassed += Time.deltaTime;
                if (OwningCharacter.Stats.ForgettingTime < _forgettingTimePassed)
                {
                    StateController.ChangeState(EnemyStateType.Patrolling);
                }
            }
        }

        public override void OnExit(EnemyStateType nextState)
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert(null != _chaseCTS, $"Chase cancellation token should be set when exiting chasing state in {OwningCharacter.ActorName}.");

            base.OnExit(nextState);

            _chaseCTS.Cancel();
            _chaseCTS.Dispose();
            _chaseCTS = null;
        }


        /****** Private Members ******/

        private ICharacter _chasingTarget;
        private CancellationTokenSource _chaseCTS;

        private float _forgettingTimePassed;

        private void SetChasingTarget()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");

            if (Perception.HasDetectedPlayer)
            {
                _chasingTarget = Perception.DetectedPlayer;
            }
            else if (null != OwningCharacter.Stats.RecentDamagedInfo)
            {
                _chasingTarget = OwningCharacter.Stats.RecentDamagedInfo.Attacker.GetComponent<ICharacter>();
                Debug.Assert(null != _chasingTarget, "Cannot find player character from recent damage info.");
            }

            Debug.Assert(null != _chasingTarget, $"Cannot find player character to chase in {OwningCharacter.ActorName}.");
        }

        private async UniTask ProgressChaseAsync()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert(null != _chaseCTS, $"Chase cancellation token is not set in {OwningCharacter.ActorName}.");

            OpResult result = await OwningCharacter.ChaseAsync(_chasingTarget, _chaseCTS.Token);
            Debug.Assert(OpResult.Failed != result, $"Chase failed.");
        }

        private void OnDestroy()
        {
            if (null != _chaseCTS)
            {
                _chaseCTS.Cancel();
                _chaseCTS.Dispose();
                _chaseCTS = null;
            }
        }
    }
}
