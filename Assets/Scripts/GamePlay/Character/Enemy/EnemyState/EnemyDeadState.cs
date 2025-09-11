using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.GamePlay
{
    public class EnemyDeadState : EnemyStateBase
    {
        public override EnemyStateType StateType => EnemyStateType.Dead;

        public override void OnEnter()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");

            base.OnEnter();

            _deadCTS = new CancellationTokenSource();
            ProgressDieAsync().Forget();
        }

        public override void OnUpdate()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");

            base.OnUpdate();
        }

        public override void OnExit(EnemyStateType nextState)
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert(null != _deadCTS, $"Dead cancellation token should be set when exiting dead state in {OwningCharacter.ActorName}.");

            base.OnExit(nextState);

            _deadCTS.Cancel();
            _deadCTS.Dispose();
            _deadCTS = null;
        }


        /****** Private Members ******/

        private CancellationTokenSource _deadCTS;

        private async UniTask ProgressDieAsync()
        {
            Debug.Assert(IsInitialized, $"{StateType} is not initialized.");
            Debug.Assert(null != _deadCTS, $"Dead cancellation token is not set in {OwningCharacter.ActorName}.");

            OwningCharacter.Movement.SetVelocity(Vector2.zero);

            OpResult result = await OwningCharacter.DieAsync(_deadCTS.Token);
            Debug.Assert(OpResult.Failed != result, "Dying failed.");
        }

        private void OnDestroy()
        {
            if (null != _deadCTS)
            {
                _deadCTS.Cancel();
                _deadCTS.Dispose();
                _deadCTS = null;
            }
        }
    }
}