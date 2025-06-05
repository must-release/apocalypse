using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine;

public class NormalInfectee : EnemyController
{
    private const int MAX_HIT_POINT = 3;
    private const float PATROL_RANGE_MAX = 30;
    private const float PATROL_RANGE_MIN = 5;
    private const float STANDING_TIME = 5f;

    private float patrolLeftEnd, patrolRightEnd; // Each end side of the patrol range
    private bool wait;
    private float waitingTime;
    private bool attacked;
    private float attackingTime;

    protected override void InitializeTerrainChecker()
    {
        groundCheckingDistance      = 3f;
        groundCheckingVector        = new Vector3(1, - 1, 0);
        ObstacleCheckingDistance    = 2f;
        checkTerrain                = true;
    }

    protected override void InitializePlayerDetector()
    {
        detectRange = new Vector2(25, 5);
        rangeOffset = new Vector2(3, 0);
    }

    protected override void InitializeDamageAndWeapon()
    {
        defaultDamageInfo   = new DamageInfo(gameObject, 1);
        weaponType          = WeaponType.Scratch;
        weaponOffset        = new Vector3(2.5f, 0, 0);
        useShortRangeWeapon = true;

        attackRange = 1.5f;
    }

    protected override void Start()
    {
        base.Start();

        MovingSpeed     = 3f;
        CurrentHitPoint = MAX_HIT_POINT;
    }

    // Set initial info for patrolling
    public override void StartPatrol()
    {
        patrolRightEnd  = transform.position.x + PATROL_RANGE_MAX / 2;
        patrolLeftEnd   = transform.position.x - PATROL_RANGE_MAX / 2;
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

    public override void StartChasing()
    {
        
    }

    // Called every frame when chasing
    public override void Chase()
    {
        // Look at the player
        int direction = ChasingTarget.position.x > transform.position.x ? 1: -1;
        if(transform.localScale.x * direction < 0) Flip();

        if (CanMoveAhead && math.abs(ChasingTarget.position.x - transform.position.x) > 0.1f) 
            enemyRigid.linearVelocity = new Vector2(direction * MovingSpeed, enemyRigid.linearVelocity.y);
        else
            enemyRigid.linearVelocity = Vector2.zero;
    }

    public override void StartAttack()
    {
        waitingTime     = 0;
        wait            = true;
        attacked        = false;
        attackingTime   = 0;
        enemyRigid.linearVelocity = Vector2.zero;
    }

    public override bool Attack()
    {
        IWeapon scratch = WeaponPool.Get();

        if ( false == attacked )
        {
            scratch.SetLocalPosition(weaponOffset);
            scratch.Attack(Vector3.zero);
            attacked = true;
        }

        // Wait a little while attacking
        attackingTime += Time.deltaTime;
        if ( attackingTime < scratch.ActiveDuration )
            return false;

        // Wait a little after attacking
        if ( wait )
        {
            waitingTime += Time.deltaTime;
            if ( scratch.PostDelay < waitingTime )
                wait = false;

            return false;
        }

        return true;
    }


    private void PatrolDirection(bool isPatrollingRight)
    {
        // There is no ground ahead, or there is obstacle ahead
        if (false == CanMoveAhead)
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

            enemyRigid.linearVelocity = Vector2.zero;
        }
        // Patrolling the area
        else if ((isPatrollingRight && transform.position.x < patrolRightEnd) ||
                (!isPatrollingRight && transform.position.x > patrolLeftEnd))
        {
            float direction = isPatrollingRight ? 1 : -1;
            enemyRigid.linearVelocity = new Vector2(direction * MovingSpeed, enemyRigid.linearVelocity.y);
        }
        // Reached one of the end side of the patrol range
        else
        {
            Flip();
            enemyRigid.linearVelocity = Vector2.zero;
        }
    }

    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, 
            transform.localScale.y, transform.localScale.z);
    }


    public override void ControlCharacter(ControlInfo controlInfo) { }
    protected override void OnAir() { }
    protected override void OnGround() { }
}