using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.GamePlay
{
    public class NormalInfectee : EnemyCharacterBase
    {
        /****** Public Members ******/

        public override async UniTask<OpResult> AttackAsync(CancellationToken cancellationToken)
        {
            Debug.Assert(null != cancellationToken, $"CancellationToken is null in AttackAsync of {ActorName}.");

            Movement.SetVelocity(Vector2.zero);  
            OpResult result = await OwningAnimator.PlayAttackAsync(cancellationToken);

            return result;
        }

        public override async UniTask<OpResult> ChaseAsync(IActor chasingTarget, CancellationToken cancellationToken)
        {
            Debug.Assert(null != chasingTarget, $"Chasing target can't be null in ChaseAsync of {ActorName}.");
            Debug.Assert(null != cancellationToken, $"CancellationToken is null in ChaseAsync of {ActorName}.");

            OwningAnimator.IsPlayerDetected = true;

            while (false == cancellationToken.IsCancellationRequested)
            {
                // Prevent oscillating near player
                if (Mathf.Abs(chasingTarget.Movement.CurrentPosition.x - Movement.CurrentPosition.x) < 0.1f)
                {
                    Movement.SetVelocity(Vector2.zero);
                    await UniTask.Yield();
                    continue;
                }

                FacingDirection chasingDirection = (Movement.CurrentPosition.x < chasingTarget.Movement.CurrentPosition.x) ? FacingDirection.Right : FacingDirection.Left;
                if (Movement.CurrentFacingDirection != chasingDirection)
                {
                    Movement.FlipFacingDirection();
                }

                int direction = (FacingDirection.Right == chasingDirection) ? 1 : -1;
                Movement.SetVelocity(Vector2.right * direction * Stats.MovingSpeed);

                await UniTask.Yield();
            }

            return cancellationToken.IsCancellationRequested ? OpResult.Aborted : OpResult.Success;
        }

        public override async UniTask<OpResult> PatrolAsync(CancellationToken cancellationToken)
        {
            Debug.Assert(null != cancellationToken, $"CancellationToken is null in PatrolAsync of {ActorName}.");

            OwningAnimator.IsPlayerDetected = false;

            while (false == cancellationToken.IsCancellationRequested)
            {
                int direction = (FacingDirection.Left == Movement.CurrentFacingDirection) ? -1 : 1;
                Movement.SetVelocity(Vector2.right * direction * Stats.MovingSpeed);

                await UniTask.Yield();
            }

            return cancellationToken.IsCancellationRequested ? OpResult.Aborted : OpResult.Success;
        }

        public override void ControlCharacter(IReadOnlyControlInfo controlInfo)
        { 
            
        }

        public void ActiveDamageArea()
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(_AttackPoint.position, new Vector2(2, 2), 0f, LayerMask.GetMask(Layer.Character));

            foreach (var hit in hits)
            {
                var player = hit.GetComponent<ICharacter>();
                if (true == player?.IsPlayer)
                {
                    player.ApplyDamage(_attackDamageInfo);
                }
            }
        }


        /****** Protected Members ******/

        protected override void OnValidate()
        {
            base.OnValidate();

            Debug.Assert(null != _AttackPoint, $"Attacking point is not assigned in {ActorName}");
        }

        protected override void Awake()
        {
            base.Awake();

            Stats.CharacterHeight = GetComponent<BoxCollider2D>().size.y * transform.localScale.y;

            _attackDamageInfo = new DamageInfo(gameObject, Stats.AttackDamage, Stats.IsContinuousAttack);
        }

        protected override void Start()
        {
            base.Start();

            _attackDamageInfo.Attacker = gameObject;
        }

        protected override void OnAir() { }
        protected override void OnGround() { }


        /****** Private Members ******/

        [SerializeField] private Transform _AttackPoint;

        private DamageInfo _attackDamageInfo;
    }
}