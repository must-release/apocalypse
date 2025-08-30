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
            Debug.Assert(null != _chasingTarget, "Cannot find player character to chase.");

            _chaseCTS = new CancellationTokenSource();
            ProgressChaseAsync().Forget();
            forgettingTime = 0;
        }

        public override void OnUpdate()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");

            base.OnUpdate();

            if (null == _chasingTarget)
            {
                StateController.ChangeState(EnemyStateType.Patrolling);
                return;
            }

            if (EnemyPerception.IsPlayerInAttackRange)
            {
                StateController.ChangeState(EnemyStateType.Attacking);
                return;
            }

            if (EnemyPerception.HasDetectedPlayer)
            {
                forgettingTime = 0;
            }
            else
            {
                forgettingTime += Time.deltaTime;
                if (_ForgetTime < forgettingTime)
                {
                    StateController.ChangeState(EnemyStateType.Patrolling);
                }
            }
        }

        public override void OnExit(EnemyStateType nextState)
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert(null != _chaseCTS, $"Chase cancellation token should be set when exiting chasing state in {EnemyCharacter.ActorName}.");

            base.OnExit(nextState);

            _chaseCTS.Cancel();
            _chaseCTS.Dispose();
            _chaseCTS = null;

            if (EnemyStateType.Attacking != nextState)
            {
                _chasingTarget = null;
            }
        }


        /****** Private Members ******/

        private const float _ForgetTime = 3f;

        private ICharacter _chasingTarget;
        private CancellationTokenSource _chaseCTS;

        private float forgettingTime;

        private void SetChasingTarget()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");

            if (EnemyPerception.HasDetectedPlayer)
            {
                _chasingTarget = EnemyPerception.DetectedPlayer;
            }
            else if (null != EnemyCharacter.Stats.RecentDamagedInfo)
            {
                _chasingTarget = EnemyCharacter.Stats.RecentDamagedInfo.Attacker.GetComponent<ICharacter>();
                Debug.Assert(null != _chasingTarget, "Cannot find player character from recent damage info.");
            }

            Debug.Assert(null != _chasingTarget, "Cannot find player character to chase.");
        }

        private async UniTask ProgressChaseAsync()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert(null != _chaseCTS, $"Chase cancellation token is not set in {EnemyCharacter.ActorName}.");

            OpResult result = await EnemyCharacter.ChaseAsync(_chasingTarget, _chaseCTS.Token);
            Debug.Assert(OpResult.Failed != result, $"Chase failed in {EnemyCharacter.ActorName}.");
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
