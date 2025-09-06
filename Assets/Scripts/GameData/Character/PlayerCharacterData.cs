using UnityEngine;

namespace AD.GamePlay
{
    [CreateAssetMenu(fileName = "NewPlayerCharacterData", menuName = "Character/PlayerCharacterData")]
    public class PlayerCharacterData : CharacterData
    {
        /****** Public Members ******/

        public float JumpingSpeed   => _jumpingSpeed;
        public float PushingSpeed   => _pushingSpeed;
        public float Gravity        => _gravity;
        

        /****** Protected Members ******/

        protected override void OnValidate()
        {
            base.OnValidate();

            Debug.Assert(0 < _jumpingSpeed);
            Debug.Assert(0 < _gravity);
        }


        /****** Private Members ******/

        [SerializeField] private float _jumpingSpeed;
        [SerializeField] private float _pushingSpeed;
        [SerializeField] private float _gravity;
    }
}