using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.GamePlay
{
    public class EnemyIdleState : EnemyStateBase
    {
        /****** Public Members ******/

        public override EnemyStateType StateType => EnemyStateType.Idle;

        public override void OnEnter()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");

            base.OnEnter();

            _idleCTS = new CancellationTokenSource();
            ProgressIdleAsync().Forget();
        }

        public override void OnUpdate()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");

            base.OnUpdate();

            if (CanChaseDetectedPlayer())
            {
                StateController.ChangeState(EnemyStateType.Chasing);
            }
        }

        public override void OnExit(EnemyStateType nextState)
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert(null != _idleCTS, $"Idle cancellation token should be set when exiting idle state in {OwningCharacter.ActorName}.");

            base.OnExit(nextState);

            _idleCTS.Cancel();
            _idleCTS.Dispose();
            _idleCTS = null;
        }
 

        /****** Private Members ******/

        private CancellationTokenSource _idleCTS;

        private async UniTask ProgressIdleAsync()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert(null != _idleCTS, $"Idle cancellation token is not set in {OwningCharacter.ActorName}.");

            OwningCharacter.Movement.SetVelocity(Vector2.zero);

            bool cancelled = await UniTask.WaitForSeconds(OwningCharacter.Stats.WaitingTime, cancellationToken: _idleCTS.Token).SuppressCancellationThrow();
            if (false == cancelled)
            {
                OwningCharacter.Movement.FlipFacingDirection();
                StateController.ChangeState(EnemyStateType.Patrolling);
            }
        }

        private bool CanChaseDetectedPlayer()
        {
            if (false == Perception.HasDetectedPlayer)
                return false;

            FacingDirection chasingDirection = (OwningCharacter.Movement.CurrentPosition.x < Perception.DetectedPlayer.Movement.CurrentPosition.x) ? FacingDirection.Right : FacingDirection.Left;
            if (OwningCharacter.Movement.CurrentFacingDirection == chasingDirection && false == Perception.CanMoveAhead)
                return false;

            return true;
        }

        private void OnDestroy()
        {
            if (null != _idleCTS)
            {
                _idleCTS.Cancel();
                _idleCTS.Dispose();
                _idleCTS = null;
            }
        }
    }
}