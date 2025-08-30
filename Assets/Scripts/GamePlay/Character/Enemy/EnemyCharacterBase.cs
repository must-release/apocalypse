using UnityEngine;

namespace AD.GamePlay
{
    [RequireComponent(typeof(Animator))]
    public class EnemyCharacterBase : CharacterBase
    {
        /****** Public Members ******/

        public override bool IsPlayer => false;

        public override void ControlCharacter(IReadOnlyControlInfo controlInfo)
        {

        }

        public override void OnDamaged(DamageInfo damageInfo)
        {

        }


        /****** Protected Members ******/

        protected Animator EnemyAnimator => _enemyAnimtaor;

        protected override void Awake()
        {
            base.Awake();

            _enemyAnimtaor = GetComponent<Animator>();
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

        private Animator _enemyAnimtaor;


    }
}
