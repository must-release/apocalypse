using UnityEngine;

namespace AD.GamePlay
{
    [CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        /****** Public Members ******/

        public float MovingSpeed => _movingSpeed;
        public float JumpingSpeed => _jumpingSpeed;
        public float Gravity => _gravity;
        public int MaxHitPoint => _maxHitPoint;


        /****** Protected Members ******/
        
        protected virtual void OnValidate()
        {
            Debug.Assert(0 < _movingSpeed);
            Debug.Assert(0 < _jumpingSpeed);
            Debug.Assert(0 < _gravity);
            Debug.Assert(0 < _maxHitPoint);
        }


        /****** Private Members ******/

        [SerializeField] private float _movingSpeed;
        [SerializeField] private float _jumpingSpeed;
        [SerializeField] private float _gravity;
        [SerializeField] private int _maxHitPoint;
    }
}