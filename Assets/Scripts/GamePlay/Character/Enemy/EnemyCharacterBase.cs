using UnityEngine;

namespace AD.GamePlay
{
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
    }
}
