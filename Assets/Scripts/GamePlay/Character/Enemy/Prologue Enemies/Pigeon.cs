using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class Pigeon : EnemyController
{
    /****** Public Members ******/

    public override float   MovingSpeed => 2f;
    public override int     MaxHitPoint => 2;

    public override void StartPatrol()
    {
        _patrolRightEnd = CurrentPosition.x + _MaxPatrolRange / 2;
        _patrolLeftEnd  = CurrentPosition.x - _MaxPatrolRange / 2; 
        _waitingTime    = 0;
        _isWaiting      = false;

        _pigeonCollider.isTrigger = false;
    }

    public override void Patrol()
    {
        if (_isWaiting)
        {
            _waitingTime += Time.deltaTime;
            if (_StandingTime < _waitingTime)
            {
                _isWaiting      = false;
                _waitingTime    = 0;

                Flip();
            }

            return;
        }

        if (false == CanMoveAhead)
        {
            UpdatePatrolRange();
        }
        else if ((FacingDirection.Right == CurrentFacingDirection && CurrentPosition.x < _patrolRightEnd) ||
                (FacingDirection.Left == CurrentFacingDirection && CurrentPosition.x > _patrolLeftEnd))
        {
            float direction = (FacingDirection.Right == CurrentFacingDirection) ? 1 : -1;
            SetVelocity(new Vector2(direction * MovingSpeed, CurrentVelocity.y));
        }
        else
        {
            // Reached one of the end side of the patrol range
            Flip();
            SetVelocity(Vector2.zero);
        }
    }

    public override void StartChasing()
    {
        _pigeonCollider.isTrigger = true;

        SetGravityScale(0);
    }

    public override void Chase()
    {
        FacingDirection direction = (ChasingTarget.position.x < CurrentPosition.x) ?  FacingDirection.Left : FacingDirection.Right;
        if (direction != CurrentFacingDirection) SetFacingDirection(direction);

        MoveTowardPlayer();
    }

    public override void StartAttack()
    {
        _waitingTime        = 0;
        _isWaiting          = true;
        _isAttackFinished   = false;
        _attackingTime      = 0;

        SetVelocity(Vector2.zero);
    }

    public override bool Attack()
    {
        IWeapon scratch = weapons.Peek();

        if (false == _isAttackFinished)
        {
            scratch.SetLocalPosition(weaponOffset);
            scratch.Attack(Vector3.zero);
            _isAttackFinished = true;
        }

        // Wait a little while attacking
        _attackingTime += Time.deltaTime;
        if (_attackingTime < scratch.ActiveDuration)
            return false;

        // Wait a little after attacking
        if (_isWaiting)
        {
            _waitingTime += Time.deltaTime;
            if (scratch.PostDelay < _waitingTime)
                _isWaiting = false;

            return false;
        }

        return true;
    }

    public override void ControlCharacter(ControlInfo controlInfo) { }


    /****** Protected Members ******/

    protected override void InitializeTerrainChecker()
    {
        groundCheckingDistance = 3f;
        groundCheckingVector = new Vector3(1, -1, 0);
        ObstacleCheckingDistance = 2f;
        checkTerrain = true;
    }

    protected override void InitializePlayerDetector()
    {
        detectRange = new Vector2(40, 40);
        rangeOffset = new Vector2(0, 0);
    }

    protected override void InitializeDamageAndWeapon()
    {
        defaultDamageInfo = new DamageInfo(gameObject, 1);
        weaponType = WeaponType.Scratch;
        weapons = new Queue<IWeapon>();
        aimingDots = new List<AimingDot>();
        weaponOffset = new Vector3(2.5f, 0, 0);
        useShortRangeWeapon = true;
        weaponCount = 1;
        aimingDotsCount = 0;

        attackRange = 1.5f;
    }

    protected override void Start()
    {
        base.Start();

        _pigeonCollider = GetComponent<Collider2D>();
        CurrentHitPoint = MaxHitPoint;
    }


    protected override void OnAir() { }
    protected override void OnGround() { }


    /****** Private Members ******/

    private const float _MaxVerticalDistanceFromPlayer      = 5f;
    private const float _MinVerticalDistanceFromPlayer      = 4f;
    private const float _MaxHorizontalDistanceFromPlayer    = 5f;
    private const float _MinHorizontalDistanceFromPlayer    = 4f;

    private const float _MaxPatrolRange = 10f;
    private const float _MinPatrolRange = 5f;
    private const float _StandingTime   = 5f;


    private Collider2D _pigeonCollider;

    private float   _patrolLeftEnd, _patrolRightEnd; // Each end side of the patrol range
    private bool    _isWaiting;
    private float   _waitingTime;
    private bool    _isAttackFinished;
    private float   _attackingTime;

    private void UpdatePatrolRange()
    {
        // Update patrol end
        if (FacingDirection.Right == CurrentFacingDirection) _patrolRightEnd = CurrentPosition.x;
        else _patrolLeftEnd = CurrentPosition.x;

        // Check if patrol area is too small
        if (_patrolRightEnd - _patrolLeftEnd < _MinPatrolRange) _isWaiting = true;
        else Flip();

        // Update other end
        if (FacingDirection.Right == CurrentFacingDirection) _patrolLeftEnd = _patrolRightEnd - _MaxPatrolRange;
        else _patrolRightEnd = _patrolLeftEnd + _MaxPatrolRange;

        SetVelocity(Vector2.zero);
    }

    private void Flip()
    {
        FacingDirection direction = (CurrentFacingDirection == FacingDirection.Left) ? FacingDirection.Right : FacingDirection.Left;
        SetFacingDirection(direction);
    }

    private void MoveTowardPlayer()
    {
        Assert.IsTrue(ChasingTarget != null, "Chasing target is null. Can't move toward the Player");

        Vector2 newVelocity = CurrentVelocity;

        float verticalDistance      = Mathf.Abs(CurrentPosition.y - ChasingTarget.position.y);
        float horizontalDistance    = Mathf.Abs(CurrentPosition.x - ChasingTarget.position.x);

        int verticalDirection   = (CurrentPosition.y < ChasingTarget.position.y) ? 1 : -1;
        int horizontalDirection = (CurrentPosition.x < ChasingTarget.position.x) ? 1 : -1;

        if (verticalDistance < _MinVerticalDistanceFromPlayer)
        {
            newVelocity.y = MovingSpeed;
        }
        else if (verticalDistance > _MaxVerticalDistanceFromPlayer)
        {
            newVelocity.y = -MovingSpeed;
        }
        else
        {
            newVelocity.y = 0;
        }

        if (horizontalDistance < _MinHorizontalDistanceFromPlayer)
        {
            newVelocity.x = -horizontalDirection * MovingSpeed;
        }
        else if (horizontalDistance > _MaxHorizontalDistanceFromPlayer)
        {
            newVelocity.x = horizontalDirection * MovingSpeed;
        }
        else
        {
            newVelocity.x = 0;
        }

        SetVelocity(newVelocity);
    }
}