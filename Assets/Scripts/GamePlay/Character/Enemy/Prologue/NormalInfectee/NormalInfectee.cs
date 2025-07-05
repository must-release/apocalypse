using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine;

public class NormalInfectee : EnemyController
{
    /****** Public Members ******/

    public override void StartPatrol()
    {
        patrolRightEnd  = transform.position.x + PATROL_RANGE_MAX / 2;
        patrolLeftEnd   = transform.position.x - PATROL_RANGE_MAX / 2;
        waitingTime = 0;
        wait = false;
    }

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
        enemyRigid.linearVelocity = Vector2.zero;

        _scratch.gameObject.SetActive(true);
        ActiveDamageArea();
    }

    public override bool Attack()
    {
        if ( wait )
        {
            waitingTime += Time.deltaTime;
            if (2 < waitingTime )
                wait = false;

            return false;
        }

        _scratch.gameObject.SetActive(false);

        return true;
    }

    public override void ControlCharacter(IReadOnlyControlInfo controlInfo) { }


    /****** Protected Members ******/

    protected override void Awake()
    {
        base.Awake();

        CharacterHeight = GetComponent<BoxCollider2D>().size.y * transform.localScale.y;
    }

    protected override void Start()
    {
        base.Start();

        MovingSpeed = 3f;
        CurrentHitPoint = MAX_HIT_POINT;

        _attackDamageInfo.Attacker = gameObject;

    }

    protected override void InitializeTerrainChecker()
    {
        groundCheckingDistance = 3f;
        groundCheckingVector = new Vector3(1, -1, 0);
        ObstacleCheckingDistance = 2f;
        checkTerrain = true;
    }

    protected override void InitializePlayerDetector()
    {
        detectRange = new Vector2(10, 2);
        rangeOffset = new Vector2(3, 0);
    }

    protected override void InitializeDamageAndWeapon()
    {
        defaultDamageInfo = new DamageInfo(gameObject, 1);
        weaponOffset = new Vector3(2.5f, 0, 0);
        useShortRangeWeapon = true;
        weaponType = ProjectileType.ProjectileTypeCount;

        attackRange = 1.5f;
    }

    protected override void OnAir() { }
    protected override void OnGround() { }


    /****** Private Members ******/

    //TODO: scratch should be controlled in animator
    [SerializeField] Transform _scratch;

    private DamageInfo _attackDamageInfo = new DamageInfo();

    private const int MAX_HIT_POINT = 3;
    private const float PATROL_RANGE_MAX = 30;
    private const float PATROL_RANGE_MIN = 5;
    private const float STANDING_TIME = 5f;

    private float patrolLeftEnd, patrolRightEnd; // Each end side of the patrol range
    private bool wait;
    private float waitingTime;

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

    private void ActiveDamageArea()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(_scratch.position, new Vector2(2,2), 0f, 1 << LayerMask.NameToLayer(Layer.Character));

        foreach (var hit in hits)
        {
            var player = hit.GetComponent<ICharacter>();
            if (true == player.IsPlayer)
            {
                player.OnDamaged(_attackDamageInfo);
            }
        }
    }
}