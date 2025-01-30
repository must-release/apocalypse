using CharacterEums;
using UnityEngine;

public class PatrollingState : EnemyStateBase
{
    protected override void StartEnemyState()
    {

    }

    public override ENEMY_STATE GetState() { return ENEMY_STATE.PATROLLING; }

    public override void OnEnter()
    {
        // Set initial info for patrolling
        enemyController.SetPatrolInfo();
    }

    public override void OnUpdate()
    {
        // Circle patrol area
        enemyController.Patrol();
    }

    public override void OnExit(ENEMY_STATE _)
    {

    }

    public override void DetectedPlayer()
    {
        enemyController.ChangeState(ENEMY_STATE.CHASING);
    }
}
