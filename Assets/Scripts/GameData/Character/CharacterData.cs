using UnityEngine;

namespace AD.GamePlay
{
    public class CharacterData : ScriptableObject
    {
        /****** Public Members ******/

        public float MovingSpeed => _movingSpeed;
        public int MaxHitPoint => _maxHitPoint;


        /****** Protected Members ******/
        
        protected virtual void OnValidate()
        {
            Debug.Assert(0 < _movingSpeed);
            Debug.Assert(0 < _maxHitPoint);
        }


        /****** Private Members ******/

        [SerializeField] private float _movingSpeed;
        [SerializeField] private int _maxHitPoint;
    }
}