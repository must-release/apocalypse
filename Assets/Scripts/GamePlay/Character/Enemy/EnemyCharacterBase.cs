using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AD.GamePlay
{
    [RequireComponent(typeof(EnemyAnimator))]
    public abstract class EnemyCharacterBase : CharacterBase, IEnemyCharacter
    {
        /****** Public Members ******/

        public new EnemyCharacterStats Stats => base.Stats as EnemyCharacterStats;
        public override bool IsPlayer => false;

        public override void ControlCharacter(IReadOnlyControlInfo controlInfo)
        {

        }

        public override void ApplyDamage(DamageInfo damageInfo)
        {
            base.ApplyDamage(damageInfo);
        }

        public abstract UniTask<OpResult> AttackAsync(CancellationToken cancellationToken);
        public abstract UniTask<OpResult> ChaseAsync(IActor chasingTarget, CancellationToken cancellationToken);
        public abstract UniTask<OpResult> PatrolAsync(CancellationToken cancellationToken);
        public abstract UniTask<OpResult> DieAsync(CancellationToken cancellationToken);


        /****** Protected Members ******/

        protected EnemyAnimator OwningAnimator => _owningAnimtaor;

        protected override void OnValidate()
        {
            base.OnValidate();

            Debug.Assert(null != _enemyHitbox, $"Enemey hitbox is not set in {ActorName}.");
        }

        protected override void Awake()
        {
            base.Awake();

            _owningAnimtaor = GetComponent<EnemyAnimator>();

            DamageInfo defaultDamage = new DamageInfo(gameObject, 1, true);
            _enemyHitbox.SetDamageArea(GetComponent<Collider2D>(), defaultDamage, true, LayerMask.NameToLayer(Layer.Default));
        }

        protected override CharacterStats CreateStats(CharacterData data)
        {
            Debug.Assert(data is EnemyCharacterData, $"data is not a enemy character data in {ActorName}.");

            // Do not call base function, because it will create a CharacterStats instance.

            return new EnemyCharacterStats(data as EnemyCharacterData);
        }

        protected override void OnAir()
        {

        }

        protected override void OnGround()
        {

        }


        /****** Private Members *******/

        [SerializeField] private DamageArea _enemyHitbox;

        private EnemyAnimator _owningAnimtaor;
    }
}
