using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Dove : EnemyController
{
    private Rigidbody2D enemyRigid; // Enemy Rigidbody2D component, used for controlling physics-based movement.
    private float movingSpeed; // Speed at which the enemy moves when following the player.
    private int movingdirection; // Direction of enemy movement: 1 for right, -1 for left.

    public float floatHeight = 5f; // Height above the player that the enemy maintains.
    public float followDistance = 7f; // Horizontal distance within which the enemy follows the player.
    public float patrolRange = 3f; // The range within which the enemy will patrol horizontally while following the player.
    public float decisionInterval = 1f; // Time interval in seconds to decide movement direction.

    private Vector2 initialLocalPosition; // Initial position of the enemy, used as the reference point for movement.
    private GameObject player;
    private bool isInitialized = false; // Indicates whether the enemy has been initialized.
    private bool isPatrolling = false; // Indicates whether the enemy is in patrol mode.
    private float nextDecisionTime = 0f; // Time when the next decision should be made.

    private void Start()
    {
        initialLocalPosition = transform.localPosition; // Set the initial position (relative coordinate).
    }

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();

        enemyRigid = GetComponent<Rigidbody2D>();
        movingSpeed = 3f;
        movingdirection = 1;

        isInitialized = true;

        // Rigidbody2D settings initialization
        enemyRigid.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation.
        enemyRigid.gravityScale = 0f; // Remove gravity effect for floating enemy.
    }

    public override void UpdateEnemy()
    {
        if (!isInitialized) return; // Do not proceed if initialization is not complete.

        base.UpdateEnemy();

        if (DetectedPlayer)
        {
            FollowPlayer(); // Follow the player if detected.
        }
        else
        {
            HoverAtInitialPosition(); // Hover at the initial position when the player is not detected.
        }
    }

    private void FollowPlayer()
    {
        if (DetectedPlayer == null) return;

        // Only decide movement direction at specified intervals to prevent rapid direction changes
        if (Time.time >= nextDecisionTime)
        {
            nextDecisionTime = Time.time + decisionInterval;

            // Determine direction to move horizontally towards the player
            int direction = DetectedPlayer.transform.position.x > transform.position.x ? 1 : -1;
            float targetY = DetectedPlayer.transform.position.y + floatHeight;

            // Move towards the player's x position and then start patrolling around it
            if (!isPatrolling)
            {
                float targetX = DetectedPlayer.transform.position.x;
                Vector2 targetPosition = new Vector2(targetX, targetY);
                Vector2 directionToMove = (targetPosition - (Vector2)transform.position).normalized;
                enemyRigid.velocity = new Vector2(directionToMove.x * movingSpeed, directionToMove.y * movingSpeed);

                if (Mathf.Abs(transform.position.x - targetX) < 0.1f)
                {
                    isPatrolling = true; // Start patrolling once near the player's position
                }
            }
            else
            {
                PatrolAroundPlayer(); // Patrol around the player's position
            }

            // Update moving direction for sprite orientation
            if (direction != movingdirection)
            {
                transform.localScale = new Vector3(-transform.localScale.x,
                    transform.localScale.y, transform.localScale.z);
                movingdirection = direction;
            }
        }
    }

    private void PatrolAroundPlayer()
    {
        if (DetectedPlayer == null) return;

        float playerX = DetectedPlayer.transform.position.x;
        float targetX = playerX + movingdirection * patrolRange;
        float targetY = DetectedPlayer.transform.position.y + floatHeight;

        // Move back and forth within the patrol range around the player's x position
        Vector2 targetPosition = new Vector2(targetX, targetY);
        Vector2 directionToMove = (targetPosition - (Vector2)transform.position).normalized;
        enemyRigid.velocity = new Vector2(directionToMove.x * movingSpeed, directionToMove.y * movingSpeed);

        // Change direction when reaching patrol boundaries
        if ((movingdirection == 1 && transform.position.x >= playerX + patrolRange) ||
            (movingdirection == -1 && transform.position.x <= playerX - patrolRange))
        {
            movingdirection *= -1;
            transform.localScale = new Vector3(-transform.localScale.x,
                transform.localScale.y, transform.localScale.z);
        }
    }

    private void HoverAtInitialPosition()
    {
        // Hover at the initial local position
        Vector2 targetPosition = new Vector2(initialLocalPosition.x, initialLocalPosition.y + floatHeight);
        Vector2 directionToMove = (targetPosition - (Vector2)transform.position).normalized;
        enemyRigid.velocity = new Vector2(directionToMove.x * movingSpeed, directionToMove.y * movingSpeed);
    }

    public override void ControlCharacter(ControlInfo controlInfo) { return; }
    public override void OnAir() { return; }
    public override void OnGround() { return; }
    public override void OnDamaged() { return; }
}
