using UnityEngine;

public class PatrollingState : EnemyStateBase
{
    protected override void StartEnemyState()
    {

    }

    public override EnemyState GetState() { return EnemyState.Patrolling; }

    public override void OnEnter()
    {
        // Set initial info for patrolling
        enemyController.StartPatrol();
    }

    public override void OnUpdate()
    {
        // Circle patrol area
        enemyController.Patrol();
    }

    public override void OnExit(EnemyState _)
    {

    }

    public override void DetectedPlayer()
    {
        enemyController.ChangeState(EnemyState.Chasing);
    }
}
