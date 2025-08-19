using UnityEngine;

public class AttackingState : EnemyStateBase
{
    public override EnemyState GetState() => EnemyState.Attacking;

    public override void OnEnter()
    {
        enemyController.StartAttack();
    }

    public override void OnUpdate()
    {
        if( enemyController.Attack() )
            enemyController.ChangeState(EnemyState.Chasing);
    }

    public override void OnExit(EnemyState _)
    {

    }

    public override void DetectedPlayer() { }
}
