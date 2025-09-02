using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.GamePlay
{
    [RequireComponent(typeof(Animator), typeof(CharacterMovement))]
    public class EnemyAnimator : MonoBehaviour
    {
        /****** Public Members ******/

        public bool IsPlayerDetected { set { _animator.SetBool(_IsPlayerDetectedParam, value); } }

        public async UniTask<OpResult> PlayAttackAsync(CancellationToken cancellationToken)
        {
            _animator.SetTrigger(_AttackTriggerParam);
            _animator.Update(0.0f);

            bool isCanceled = await UniTask.WaitUntil(() => 1f < _animator.GetCurrentAnimatorStateInfo(0).normalizedTime, cancellationToken: cancellationToken).SuppressCancellationThrow();

            return isCanceled ? OpResult.Aborted : OpResult.Success;
        }

        public async UniTask<OpResult> PlayDeathAsync(CancellationToken cancellationToken)
        {
            _animator.SetTrigger(_DeathTriggerParam);
            _animator.Update(0.0f);

            bool isCanceled = await UniTask.WaitUntil(() => 1f < _animator.GetCurrentAnimatorStateInfo(0).normalizedTime, cancellationToken: cancellationToken).SuppressCancellationThrow();

            return isCanceled ? OpResult.Aborted : OpResult.Success;
        }

        /****** Private Members ******/

        private CharacterMovement   _enemyMovement;
        private Animator            _animator;

        private const string _AttackTriggerParam    = "AttackTrigger";
        private const string _MovingSpeedParam      = "MovingSpeed";
        private const string _IsPlayerDetectedParam = "IsPlayerDetected";
        private const string _DeathTriggerParam     = "DeathTrigger";

        
        private void Awake()
        {
            _enemyMovement  = GetComponent<CharacterMovement>();
            _animator       = GetComponent<Animator>();

            CheckAnimatorParams();
        }

        private void Update()
        {
            _animator.SetFloat(_MovingSpeedParam, _enemyMovement.CurrentVelocity.magnitude);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void CheckAnimatorParams()
        {
            Debug.Assert(HasParam(_AttackTriggerParam, AnimatorControllerParameterType.Trigger), $"{_animator.name} doesn't have AttackTrigger param.");
            Debug.Assert(HasParam(_MovingSpeedParam, AnimatorControllerParameterType.Float), $"{_animator.name} doesn't have MovingSpeed param.");
            Debug.Assert(HasParam(_IsPlayerDetectedParam, AnimatorControllerParameterType.Bool), $"{_animator.name} doesn't have IsPlayerDetected param.");
            Debug.Assert(HasParam(_DeathTriggerParam, AnimatorControllerParameterType.Trigger), $"{_animator.name} doesn't have DeathTrigger param.");
        }
        
        private bool HasParam(string paramName, AnimatorControllerParameterType paramType)
        {
            foreach (AnimatorControllerParameter parameter in _animator.parameters)
            {
                if (parameter.name == paramName && parameter.type == paramType)
                    return true;
            }
            return false;
        }
    }
}