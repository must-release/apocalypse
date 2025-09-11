using UnityEngine;
using Unity.Mathematics;
using AD.GamePlay;

namespace AD.GamePlay
{
    [RequireComponent(typeof(Collider2D))]
    public class EnemyPerception : MonoBehaviour
    {
        /****** Public Members ******/

        public ICharacter DetectedPlayer    { get; private set; }
        public bool IsPlayerInAttackRange   => CheckIsPlayerInAttackRange();
        public bool HasDetectedPlayer       => null != DetectedPlayer;
        public bool CanMoveAhead            => CheckIsGroundAhead() && false == CheckIsObstacleAhead();

        public void InitializePerception(IEnemyCharacter enemyCharacter)
        {
            Debug.Assert(null != enemyCharacter, "Enemy character is null.");

            _enemyMovement  = enemyCharacter.Movement;
            _enemyStats     = enemyCharacter.Stats;
            
            _groundCheckingVector       = _enemyStats.GroundCheckingVector;
            _groundCheckingDistance     = _enemyStats.GroundCheckingDistance;
            _obstacleCheckingDistance   = _enemyStats.ObstacleCheckingDistance;

            SetupDetectionCollider();
            _isInitialized = true;
        }


        /****** Private Members ******/

        private CharacterMovement _enemyMovement;
        private EnemyCharacterStats _enemyStats;
        
        private Vector3 _groundCheckingVector;
        private float _groundCheckingDistance;
        private float _obstacleCheckingDistance;
        private bool _isInitialized;

        private bool CheckIsGroundAhead()
        {
            Debug.Assert(_isInitialized, "Enemy perception is not initialized.");

            _groundCheckingVector.x = (FacingDirection.Right == _enemyMovement.CurrentFacingDirection)
                ? math.abs(_groundCheckingVector.x)
                : -math.abs(_groundCheckingVector.x);

            var groundHit = Physics2D.Raycast(_enemyMovement.CurrentPosition, _groundCheckingVector, _groundCheckingDistance, LayerMask.GetMask(Layer.Ground));

            return groundHit.collider != null;
        }

        private bool CheckIsObstacleAhead()
        {
            Debug.Assert(_isInitialized, "Enemy perception is not initialized.");

            int direction = (FacingDirection.Right == _enemyMovement.CurrentFacingDirection) ? 1 : -1;
            _groundCheckingVector.x = (FacingDirection.Right == _enemyMovement.CurrentFacingDirection)
                ? math.abs(_groundCheckingVector.x)
                : -math.abs(_groundCheckingVector.x);

            var frontHit = Physics2D.Raycast(_enemyMovement.CurrentPosition, Vector3.right * direction, _obstacleCheckingDistance, LayerMask.GetMask(Layer.Obstacle, Layer.Ground));
            var bottomHit = Physics2D.Raycast(_enemyMovement.CurrentPosition, _groundCheckingVector, _groundCheckingDistance, LayerMask.GetMask(Layer.Obstacle));

            return frontHit.collider != null || bottomHit.collider != null;
        }

        private bool CheckIsPlayerInAttackRange()
        {
            Debug.Assert(_isInitialized, "Enemy perception is not initialized.");

            if (false == HasDetectedPlayer)
                return false;

            float distanceToPlayer = Vector3.Distance(_enemyMovement.CurrentPosition, DetectedPlayer.Movement.CurrentPosition);
            return distanceToPlayer <= _enemyStats.AttackRange;
        }

        private void SetupDetectionCollider()
        {
            Debug.Assert(_isInitialized == false, "SetupDetectionCollider should be called during initialization.");

            var collider2D = GetComponent<Collider2D>();
            collider2D.gameObject.layer = LayerMask.NameToLayer(Layer.Default);

            if (collider2D is BoxCollider2D boxCollider)
            {
                boxCollider.size = _enemyStats.DetectRange;
                boxCollider.offset = _enemyStats.RangeOffset;
            }
            else if (collider2D is CircleCollider2D circleCollider)
            {
                circleCollider.radius = math.max(_enemyStats.DetectRange.x, _enemyStats.DetectRange.y) * 0.5f;
                circleCollider.offset = _enemyStats.RangeOffset;
            }
            else
            {
                Logger.Write(LogCategory.GamePlay, $"{collider2D} is not supported in enemy perception.", isDebugOnly: true);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<ICharacter>(out var character) && character.IsPlayer)
            {
                DetectedPlayer = character;
            }       
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (false == other.CompareTag("Player"))
                return;
        
            DetectedPlayer = null;       
        }

        private void OnDrawGizmosSelected()
        {
            if (false == _isInitialized)
                return;

            int direction = (FacingDirection.Right == _enemyMovement.CurrentFacingDirection) ? 1 : -1;
            _groundCheckingVector.x = (FacingDirection.Right == _enemyMovement.CurrentFacingDirection)
                ? math.abs(_groundCheckingVector.x)
                : -math.abs(_groundCheckingVector.x);

            Gizmos.color = Color.red;
            Vector3 start = _enemyMovement.CurrentPosition;
            Vector3 end = start + _groundCheckingVector.normalized * _groundCheckingDistance;
            Gizmos.DrawLine(start, end);

            end = start + Vector3.right * direction * _obstacleCheckingDistance;
            Gizmos.DrawLine(start, end);
        }
    }
}