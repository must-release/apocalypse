using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

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

            InitPatrolRange();
            ProgressPatrolAsync().Forget();
        }

        public override void OnUpdate()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");

            base.OnUpdate();

            if (Perception.HasDetectedPlayer)
            {
                StateController.ChangeState(EnemyStateType.Chasing);
            }
            else if (false == Perception.CanMoveAhead)
            {
                UpdatePatrolRange();
                if (CheckIfPatrolRangeIsTooShort())
                {
                    StateController.ChangeState(EnemyStateType.Idle);
                }
                else
                {
                    OwningCharacter.Movement.FlipFacingDirection();
                }
            }
        }

        public override void OnExit(EnemyStateType nextState)
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert(null != _patrolCTS, $"Patrol cancellation token should be set when exiting patrolling state in {OwningCharacter.ActorName}.");

            base.OnExit(nextState);

            _patrolCTS.Cancel();
            _patrolCTS.Dispose();
            _patrolCTS = null;
        }


        /****** Private Members ******/

        private CancellationTokenSource _patrolCTS;

        private float _patrolLeftEnd    = 0;
        private float _patrolRightEnd   = 0;

        private async UniTask ProgressPatrolAsync()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert(null != _patrolCTS, $"Patrol cancellation token is not set in {OwningCharacter.ActorName}.");

            OpResult result = await OwningCharacter.PatrolAsync(_patrolCTS.Token);
            Debug.Assert(OpResult.Failed != result, $"Patrol failed.");
        }

        private void InitPatrolRange()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert((int)FacingDirection.FacingDirectionCount == 2, "Check here when adding new facing directions.");

            if (FacingDirection.Left == OwningCharacter.Movement.CurrentFacingDirection)
            {
                _patrolLeftEnd = OwningCharacter.Movement.CurrentPosition.x - OwningCharacter.Stats.MaxPatrolRange / 2;
                _patrolRightEnd = OwningCharacter.Movement.CurrentPosition.x;
            }
            else
            {
                _patrolRightEnd = OwningCharacter.Movement.CurrentPosition.x + OwningCharacter.Stats.MaxPatrolRange / 2;
                _patrolLeftEnd = OwningCharacter.Movement.CurrentPosition.x;
            }
        }

        private void UpdatePatrolRange()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert((int)FacingDirection.FacingDirectionCount == 2, "Check here when adding new facing directions.");
            Debug.Assert(false == Perception.CanMoveAhead, "Update patrol range only when enemy can't move ahead");

            if (FacingDirection.Left == OwningCharacter.Movement.CurrentFacingDirection)
            {
                _patrolLeftEnd = OwningCharacter.Movement.CurrentPosition.x;
            }
            else
            {
                _patrolRightEnd = OwningCharacter.Movement.CurrentPosition.x;
            }
        }

        private bool CheckIfPatrolRangeIsTooShort()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert(false == Perception.CanMoveAhead, "Check patrol range only when enemy can't move ahead");

            float oppositeSidePos = (FacingDirection.Left == OwningCharacter.Movement.CurrentFacingDirection) ? _patrolRightEnd : _patrolLeftEnd;
            return Mathf.Abs(oppositeSidePos - OwningCharacter.Movement.CurrentPosition.x) < OwningCharacter.Stats.MinPatrolRange;
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