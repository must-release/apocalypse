using UnityEngine;

namespace AD.GamePlay
{
    public class EnemyCharacterStats : CharacterStats
    {
        /****** Public Members ******/

        public Vector2 DetectRange { get; private set; }
        public Vector2 RangeOffset { get; private set; }
        public float AttackRange { get; private set; }
        public int AttackDamage { get; private set; }
        public bool IsContinuousAttack { get; private set; }
        public float MaxPatrolRange { get; private set; }
        public float MinPatrolRange { get; private set; }
        public float WaitingTime { get; private set; }
        public float ForgettingTime { get; private set; }
        public float GroundCheckingDistance { get; private set; }
        public float ObstacleCheckingDistance { get; private set; }
        public Vector3 GroundCheckingVector { get; private set; }


        /****** Private Members ******/

        public EnemyCharacterStats(EnemyCharacterData enemyData) : base(enemyData)
        {
            Debug.Assert(null != enemyData, "EnemyCharacterData cannot be null");

            DetectRange                 = enemyData.DetectRange;
            RangeOffset                 = enemyData.RangeOffset;
            AttackRange                 = enemyData.AttackRange;
            AttackDamage                = enemyData.AttackDamage;
            IsContinuousAttack          = enemyData.IsContinuousAttack;
            MaxPatrolRange              = enemyData.MaxPatrolRange;
            MinPatrolRange              = enemyData.MinPatrolRange;
            WaitingTime                 = enemyData.WaitingTime;
            ForgettingTime              = enemyData.ForgettingTime;
            GroundCheckingDistance      = enemyData.GroundCheckingDistance;
            ObstacleCheckingDistance    = enemyData.ObstacleCheckingDistance;
            GroundCheckingVector        = enemyData.GroundCheckingVector;
        }
    }
}