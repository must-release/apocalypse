using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace AD.GamePlay
{
    public class NormalInfectee : EnemyCharacterBase, IEnemyCharacter
    {
        /****** Public Members ******/

        public new EnemyCharacterStats Stats => base.Stats as EnemyCharacterStats;

        public UniTask<OpResult> AttackAsync(CancellationToken cancellationToken)
        {
            return UniTask.FromResult(OpResult.Success);
        }

        public async UniTask<OpResult> ChaseAsync(IActor chasingTarget, CancellationToken cancellationToken)
        {
            Debug.Assert(null != chasingTarget, $"Chasing target can't be null in ChaseAsync of {ActorName}.");
            Debug.Assert(null != cancellationToken, $"CancellationToken is null in ChaseAsync of {ActorName}.");

            EnemyAnimator.Play("Normal_Infectee_Patrolling");

            while (false == cancellationToken.IsCancellationRequested)
            {
                int direction = chasingTarget. > transform.position.x ? 1 : -1;
                if (0 < transform.localScale.x * direction) Flip();

                if (CanMoveAhead && math.abs(ChasingTarget.position.x - transform.position.x) > 0.1f)
                {
                    enemyRigid.linearVelocity = new Vector2(direction * MovingSpeed, enemyRigid.linearVelocity.y);
                    EnemyAnimator.Play("Normal_Infectee_Patrolling");
                }
                else
                {
                    EnemyAnimator.Play("Normal_Infectee_Idle");
                    enemyRigid.linearVelocity = Vector2.zero;
                }
            }

            return UniTask.FromResult(OpResult.Success);
        }

        public async UniTask<OpResult> PatrolAsync(CancellationToken cancellationToken)
        {
            Debug.Assert(null != cancellationToken, $"CancellationToken is null in PatrolAsync of {ActorName}.");

            EnemyAnimator.Play("Normal_Infectee_Patrolling");

            while (false == cancellationToken.IsCancellationRequested)
            {
                int direction = (FacingDirection.Left == Movement.CurrentFacingDirection) ? -1 : 1;
                Movement.SetVelocity(Vector2.right * direction * Stats.MovingSpeed);

                await UniTask.Yield();
            }

            return OpResult.Success;
        }

        public UniTask<OpResult> DieAsync(CancellationToken cancellationToken)
        {
            EnemyAnimator.Play("Normal_Infectee_Dead");

            return UniTask.FromResult(OpResult.Success);
        }

        public override void Chase()
        {
            // Look at the player
            int direction = ChasingTarget.position.x > transform.position.x ? 1 : -1;
            if (0 < transform.localScale.x * direction) Flip();

            if (CanMoveAhead && math.abs(ChasingTarget.position.x - transform.position.x) > 0.1f)
            {
                enemyRigid.linearVelocity = new Vector2(direction * MovingSpeed, enemyRigid.linearVelocity.y);
                EnemyAnimator.Play("Normal_Infectee_Patrolling");
            }
            else
            {
                EnemyAnimator.Play("Normal_Infectee_Idle");
                enemyRigid.linearVelocity = Vector2.zero;
            }
        }

        // public override void StartAttack()
        // {
        //     EnemyAnimator.Play("Normal_Infectee_Attacking");

        //     _waitingTime = 0;
        //     _isWaiting = true;
        //     enemyRigid.linearVelocity = Vector2.zero;
        // }

        // public override bool Attack()
        // {
        //     if (_isWaiting)
        //     {
        //         _waitingTime += Time.deltaTime;
        //         if (2 < _waitingTime)
        //             _isWaiting = false;

        //         return false;
        //     }

        //     return true;
        // }


        public override void ControlCharacter(IReadOnlyControlInfo controlInfo)
        { 
            
        }


        public override void OnDamaged(DamageInfo damageInfo)
        {
            
        }

        public void ActiveDamageArea()
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(_AttackPoint.position, new Vector2(2, 2), 0f, 1 << LayerMask.NameToLayer(Layer.Character));

            foreach (var hit in hits)
            {
                var player = hit.GetComponent<ICharacter>();
                if (true == player?.IsPlayer)
                {
                    player.OnDamaged(_attackDamageInfo);
                }
            }
        }
        

        /****** Protected Members ******/

        protected override void Awake()
        {
            base.Awake();

            Stats.CharacterHeight = GetComponent<BoxCollider2D>().size.y * transform.localScale.y;
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

        private const int _MaxHitPoint = 3;
        private const float _MaxPatrolRange = 30f;
        private const float _MinPatrolRange = 3f;
        private const float _StandingTime = 2f;


        private DamageInfo _attackDamageInfo = new DamageInfo();

        private void OnValidate()
        {
            Debug.Assert(null != _AttackPoint, $"Attacking point is no assigned in {ActorName}");
        }
    }
}