using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine;

public class NormalInfectee : EnemyController
{
    /****** Public Members ******/

    public override void StartPatrol()
    {
        EnemyAnimator.Play("Normal_Infectee_Patrolling");

        patrolRightEnd  = transform.position.x + _MaxPatrolRange / 2;
        patrolLeftEnd   = transform.position.x - _MaxPatrolRange / 2;
        waitingTime = 0;
        wait = false;
    }

    public override void Patrol()
    {
        // Wait a little if patrol area is too small
        if (wait)
        {
            waitingTime += Time.deltaTime;
            if (waitingTime > _StandingTime)
            {
                wait = false;
                waitingTime = 0;

                // Flip body
                Flip();
            }
            return;
        }

        // Decide where to patrol : left or right side
        PatrolDirection(transform.localScale.x < 0);
    }

    public override void StartChasing()
    {
        EnemyAnimator.Play("Normal_Infectee_Patrolling");
    }

    public override void Chase()
    {
        // Look at the player
        int direction = ChasingTarget.position.x > transform.position.x ? 1: -1;
        if(0 < transform.localScale.x * direction) Flip();

        if (CanMoveAhead && math.abs(ChasingTarget.position.x - transform.position.x) > 0.1f) 
            enemyRigid.linearVelocity = new Vector2(direction * MovingSpeed, enemyRigid.linearVelocity.y);
        else
            enemyRigid.linearVelocity = Vector2.zero;
    }

    public override void StartAttack()
    {
        EnemyAnimator.Play("Normal_Infectee_Attacking");

        waitingTime     = 0;
        wait            = true;
        enemyRigid.linearVelocity = Vector2.zero;
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

        return true;
    }

    public override void OnDead()
    {
        EnemyAnimator.Play("Normal_Infectee_Dead");
    }


    public override void ControlCharacter(IReadOnlyControlInfo controlInfo) { }


    public void ActiveDamageArea()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(_AttackPoint.position, new Vector2(2,2), 0f, 1 << LayerMask.NameToLayer(Layer.Character));

        foreach (var hit in hits)
        {
            var player = hit.GetComponent<ICharacter>();
            if (true == player?.IsPlayer)
            {
                player.OnDamaged(_attackDamageInfo);
            }
        }
    }

    /****** Protected Members ******/

    protected override void Awake()
    {
        base.Awake();

        CharacterHeight = GetComponent<BoxCollider2D>().size.y * transform.localScale.y;
    }

    protected override void Start()
    {
        base.Start();

        MovingSpeed = 2f;
        CurrentHitPoint = _MaxHitPoint;

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

    [SerializeField] private Transform _AttackPoint;

    private const int _MaxHitPoint      = 3;
    private const float _MaxPatrolRange = 30;
    private const float _MinPatrolRange = 5;
    private const float _StandingTime   = 5f;

    
    private DamageInfo _attackDamageInfo = new DamageInfo();

    private float patrolLeftEnd, patrolRightEnd; // Each end side of the patrol range
    private bool wait;
    private float waitingTime;

    private void OnValidate()
    {
        Debug.Assert(null != _AttackPoint, $"Attacking point is no assigned in {ActorName}");
    }

    private void PatrolDirection(bool isPatrollingRight)
    {
        if (false == CanMoveAhead)
        {
            // Update patrol end
            if (isPatrollingRight) patrolRightEnd = transform.position.x;
            else patrolLeftEnd = transform.position.x;

            // Check if patrol area is too small
            if (patrolRightEnd - patrolLeftEnd < _MinPatrolRange) wait = true;
            else Flip();

            // Update other end
            if (isPatrollingRight) patrolLeftEnd = patrolRightEnd - _MaxPatrolRange;
            else patrolRightEnd = patrolLeftEnd + _MaxPatrolRange;

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
}