using Unity.Mathematics;
using UnityEngine;

public class NormalInfectee : EnemyController
{

    public float MovingSpeed;


    private Rigidbody2D enemyRigid;

    private const float PATROL_RANGE_MAX = 30;
    private const float PATROL_RANGE_MIN = 5;
    private const float STANDING_TIME = 5f;
    private float patrolLeftEnd, patrolRightEnd; // Each end side of the partrol range
    private bool wait;
    private float waitingTime;

    protected override void AwakeEnemy()
    {
        base.AwakeEnemy();

        enemyRigid = transform.GetComponent<Rigidbody2D>();

        // Terrain checker settings
        groundCheckingDistance = 5f;
        groundCheckingVector = new Vector3(1, - 1, 0);
        ObstacleCheckingDistance = 3f;
        checkTerrain = true;

        // Player detector settings
        detectRange = new Vector2(25, 5);
        rangeOffset = new Vector2(3, 0);

        attackRange = 5;
    }

    protected override void StartEnemy()
    {
        MovingSpeed = 5f;
    }

    // Set initial info for patrolling
    public override void SetPatrolInfo()
    {
        patrolRightEnd = transform.position.x + PATROL_RANGE_MAX / 2;
        patrolLeftEnd = transform.position.x - PATROL_RANGE_MAX / 2;
        waitingTime = 0;
        wait = false;
    }

    // Called every frame when patrolling
    public override void Patrol()
    {
        // Wait a little if patrol area is too small
        if (wait)
        {
            waitingTime += Time.deltaTime;
            if (waitingTime > STANDING_TIME)
            {
                wait = false;
                waitingTime = 0;

                // Flip body
                Flip();
            }
            return;
        }

        // Decide where to patrol : left or right side
        PatrolDirection(transform.localScale.x > 0);
    }

    // Called every frame when chasing
    public override void ChasePlayer()
    {
        // Look at the player
        int direction = ChasingTarget.position.x > transform.position.x ? 1: -1;
        if(transform.localScale.x * direction < 0) Flip();

        if (CanMoveAhead() && math.abs(ChasingTarget.position.x - transform.position.x) > 0.1f) 
            enemyRigid.velocity = new Vector2(direction * MovingSpeed, enemyRigid.velocity.y);
        else
            enemyRigid.velocity = Vector2.zero;
    }

    private void PatrolDirection(bool isPatrollingRight)
    {
        // There is no ground ahead, or there is obstacle ahead
        if (!CanMoveAhead())
        {
            // Update patrol end
            if (isPatrollingRight) patrolRightEnd = transform.position.x;
            else patrolLeftEnd = transform.position.x;

            // Check if patrol area is too small
            if (patrolRightEnd - patrolLeftEnd < PATROL_RANGE_MIN) wait = true;
            else Flip();

            // Update other end
            if (isPatrollingRight) patrolLeftEnd = patrolRightEnd - PATROL_RANGE_MAX;
            else patrolRightEnd = patrolLeftEnd + PATROL_RANGE_MAX;

            enemyRigid.velocity = Vector2.zero;
        }
        // Patrolling the area
        else if ((isPatrollingRight && transform.position.x < patrolRightEnd) ||
                (!isPatrollingRight && transform.position.x > patrolLeftEnd))
        {
            float direction = isPatrollingRight ? 1 : -1;
            enemyRigid.velocity = new Vector2(direction * MovingSpeed, enemyRigid.velocity.y);
        }
        // Reached one of the end side of the patrol range
        else
        {
            Flip();
            enemyRigid.velocity = Vector2.zero;
        }
    }

    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, 
            transform.localScale.y, transform.localScale.z);
    }


    public override void ControlCharacter(ControlInfo controlInfo) { return; }
    public override void OnAir() { return; }
    public override void OnGround() { return; }
    public override void OnDamaged(DamageInfo damageInfo) 
    {
        gameObject.SetActive(false);
    }
}