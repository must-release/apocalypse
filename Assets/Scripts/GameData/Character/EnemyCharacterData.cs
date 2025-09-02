using UnityEngine;

namespace AD.GamePlay
{
    [CreateAssetMenu(fileName = "NewEnemyCharacterData", menuName = "Character/EnemyCharacterData")]
    public class EnemyCharacterData : CharacterData
    {
        /****** Public Members ******/

        public Vector2 DetectRange              => _detectRange;
        public Vector2 RangeOffset              => _rangeOffset;
        public float AttackRange                => _attackRange;
        public int AttackDamage                 => _attackDamage;
        public bool IsContinuousAttack          => _isContinuousAttack;
        public float MaxPatrolRange             => _maxPatrolRange;
        public float MinPatrolRange             => _minPatrolRange;
        public float WaitingTime                => _waitingTime;
        public float ForgettingTime             => _forgettingTime;   
        public float GroundCheckingDistance     => _groundCheckingDistance;
        public float ObstacleCheckingDistance   => _obstacleCheckingDistance;
        public Vector3 GroundCheckingVector     => _groundCheckingVector;


        /****** Protected Members ******/

        protected override void OnValidate()
        {
            base.OnValidate();

            Debug.Assert(0f < _detectRange.x && 0f < _detectRange.y, "Detect range must be positive values");
            Debug.Assert(0f < _attackRange, "Attack range must be positive");
            Debug.Assert(0f < _attackDamage, "Attack damage must be greater than 0.");
            Debug.Assert(0f < _maxPatrolRange, "Max patrol range must be positive");
            Debug.Assert(_minPatrolRange < _maxPatrolRange, "Min patrol range must be larger than min patrol range.");
            Debug.Assert(0 < _waitingTime, "Waiting time must be positive");
            Debug.Assert(0f < _groundCheckingDistance, "Ground checking distance must be positive");
            Debug.Assert(0f < _obstacleCheckingDistance, "Obstacle checking distance must be positive");
        }
        

        /****** Private Members ******/

        [SerializeField] private Vector2 _detectRange = new Vector2(5f, 1f);
        [SerializeField] private Vector2 _rangeOffset = Vector2.zero;
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private int _attackDamage = 1;
        [SerializeField] private bool _isContinuousAttack = false;
        [SerializeField] private float _maxPatrolRange = 10f;
        [SerializeField] private float _minPatrolRange = 3f;
        [SerializeField] private float _waitingTime = 2f;
        [SerializeField] private float _forgettingTime = 3f;
        [SerializeField] private float _groundCheckingDistance = 2f;
        [SerializeField] private float _obstacleCheckingDistance = 1f;
        [SerializeField] private Vector3 _groundCheckingVector = new Vector3(1f, -1f, 0f);
    }
}