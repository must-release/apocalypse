// using UnityEditor.Experimental.GraphView;
// using UnityEngine;

// public class LegacyNormalInfectee : EnemyController
// {
//     private Rigidbody2D enemyRigid; // Enemy Rigidbody2D component, used for controlling physics-based movement.
//     private float movingSpeed; // Speed at which the enemy moves when patrolling or following the player.
//     private int movingdirection; // Direction of enemy movement: 1 for right, -1 for left.
//     private bool patrolling; // Indicates whether the enemy is currently patrolling.

//     public float patrolDistance = 5f; // Maximum distance the enemy can move in each direction while patrolling.
//     public Transform groundCheck; // Ground Check position for detecting if the enemy is on the ground.
//     public float groundCheckRadius = 0.1f; // Radius for ground detection.
//     public LayerMask groundLayer; // Layer used to determine what is considered ground.
//     public float initialHealth = 100f; // Stores the initial health of the enemy to determine when to chase the player.

//     private Vector2 initialLocalPosition; // Initial position of the enemy, used as the reference point for patrolling.
//     private GameObject player;
//     private bool isInitialized = false; // Indicates whether the enemy has been initialized.
//     private bool isGrounded = true; // Indicates whether the enemy is currently on the ground.
//     private bool returningToStart = false; // Indicates whether the enemy is returning to its initial position.
//     private float CurrentHealth;

//     private void Start()
//     {
//         initialLocalPosition = transform.localPosition; // Set the initial position (relative coordinate).
//         CurrentHealth = initialHealth; // Set the initial health.
//     }

//     protected override void AwakeEnemy()
//     {
//         base.AwakeEnemy();

//         enemyRigid = GetComponent<Rigidbody2D>();
//         movingSpeed = 3f;
//         movingdirection = 1;
//         patrolling = true;

//         isInitialized = true;

//         // Rigidbody2D settings initialization
//         enemyRigid.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation.
//     }

//     protected override void UpdateEnemy()
//     {
//         if (!isInitialized) return; // Do not proceed if initialization is not complete.

//         base.UpdateEnemy();

//         CheckGroundStatus(); // Check if the enemy is on the ground.

//         if (CurrentHealth < initialHealth)
//         {
//             FollowPlayer(); // Follow the player if health has dropped.
//         }
//         else if (DetectedPlayer)
//         {
//             FollowPlayer(); // Follow the player if detected.
//         }
//         else if (returningToStart)
//         {
//             ReturnToStartPosition(); // Return to the initial position.
//         }
//         else if (patrolling)
//         {
//             PatrolArea(); // Start patrolling.
//         }
//     }

//     private void CheckGroundStatus()
//     {
//         // Detect if the enemy is on the ground using the Ground Check position.
//         Collider2D groundHit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

//         if (groundHit != null)
//         {
//             // If the enemy is touching the ground.
//             isGrounded = true;
//         }
//         else
//         {
//             // If the enemy is not touching the ground or is in the air.
//             isGrounded = false;
//         }
//     }

//     private void FollowPlayer()
//     {
//         if (DetectedPlayer == null)
//         {
//             returningToStart = true;
//             patrolling = false;
//             return;
//         }

//         int direction = DetectedPlayer.transform.position.x > transform.position.x ? 1 : -1;
//         enemyRigid.velocity = new Vector2(direction * movingSpeed, enemyRigid.velocity.y);

//         if (direction != movingdirection)
//         {
//             transform.localScale = new Vector3(-transform.localScale.x,
//                 transform.localScale.y, transform.localScale.z);
//             movingdirection = direction;
//         }
//     }

//     private void ReturnToStartPosition()
//     {
//         float directionToStart = initialLocalPosition.x > transform.localPosition.x ? 1f : -1f;
//         enemyRigid.velocity = new Vector2(directionToStart * movingSpeed, enemyRigid.velocity.y);

//         if (directionToStart != movingdirection)
//         {
//             transform.localScale = new Vector3(-transform.localScale.x,
//                 transform.localScale.y, transform.localScale.z);
//             movingdirection = (int)directionToStart;
//         }

//         // Start patrolling once the enemy reaches the initial position.
//         if (Mathf.Abs(initialLocalPosition.x - transform.localPosition.x) < 0.1f)
//         {
//             transform.localPosition = initialLocalPosition; // Set the exact initial position.
//             returningToStart = false;
//             patrolling = true;
//         }
//     }

//     private void PatrolArea()
//     {
//         if (!isGrounded)
//         {
//             // Change direction if the enemy is not on the ground.
//             movingdirection *= -1;
//             transform.localScale = new Vector3(-transform.localScale.x,
//                 transform.localScale.y, transform.localScale.z);
//         }

//         // Move while patrolling.
//         float targetPositionX = initialLocalPosition.x + movingdirection * patrolDistance;
//         Vector2 targetLocalPosition = new Vector2(targetPositionX, transform.localPosition.y);
//         Vector2 direction = (targetLocalPosition - (Vector2)transform.localPosition).normalized;

//         enemyRigid.velocity = new Vector2(direction.x * movingSpeed, enemyRigid.velocity.y);

//         // Change direction when reaching patrol boundaries.
//         if ((movingdirection == 1 && transform.localPosition.x >= initialLocalPosition.x + patrolDistance) ||
//             (movingdirection == -1 && transform.localPosition.x <= initialLocalPosition.x - patrolDistance))
//         {
//             movingdirection *= -1;
//             transform.localScale = new Vector3(-transform.localScale.x,
//                 transform.localScale.y, transform.localScale.z);
//         }
//     }

//     private void OnTriggerExit2D(Collider2D other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             if (other.gameObject == DetectedPlayer)
//             {
//                 DetectedPlayer = null;
//                 returningToStart = true; // Return to the initial position after losing the player.
//                 patrolling = false;
//             }
//             else
//             {
//                 Debug.LogError("Unknown Player Exiting from Detect Range");
//             }
//         }
//     }

//     private void OnDrawGizmos()
//     {
//         if (groundCheck != null)
//         {
//             Gizmos._color = Color.red;
//             Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); // Draw ground detection range in the Scene view.
//         }
//     }

//     public override void ControlCharacter(ControlInfo controlInfo) { return; }
//     public override void OnAir() { return; }
//     public override void OnGround() { return; }
//     public override void OnDamaged() { gameObject.SetActive(false); }
// }
