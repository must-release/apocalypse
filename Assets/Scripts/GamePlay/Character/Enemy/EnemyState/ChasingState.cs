using UnityEngine;

public class ChasingState : EnemyStateBase
{
    private const float FORGET_TIME = 10f;
    private float forgettingTime;

    protected override void Awake()
    {
        base.Awake();

        forgettingTime = 0;
    }

    public override EnemyState GetState() { return EnemyState.Chasing; }

    public override void OnEnter()
    {
        if ( enemyController.DetectedPlayer )
            enemyController.ChasingTarget = enemyController.DetectedPlayer.transform;
        else if ( null != enemyController.RecentDamagedInfo )
            enemyController.ChasingTarget = enemyController.RecentDamagedInfo.Attacker.transform;
        
        if ( null == enemyController.ChasingTarget )
            Debug.LogError("There's nobody to chase!");

        enemyController.StartChasing();

        forgettingTime = 0;
    }

    public override void OnUpdate()
    {
        if (null == enemyController.ChasingTarget)
        {
            enemyController.ChangeState(EnemyState.Patrolling);
            return;
        }

        // Chase detected player
        enemyController.Chase();

        if ( enemyController.IsPlayerInAttackRange )
        {
            enemyController.ChangeState(EnemyState.Attacking); 
            return;
        }

        // Forget player when player is not detected for a while
        if ( null == enemyController.DetectedPlayer )
        {
            forgettingTime += Time.deltaTime;
            if (forgettingTime > FORGET_TIME)
                enemyController.ChangeState(EnemyState.Patrolling);
        }
        else
        {
            forgettingTime = 0;
        }
    }

    public override void OnExit(EnemyState nextState)
    {
        if ( EnemyState.Attacking != nextState)
            enemyController.ChasingTarget = null;
    }

    public override void DetectedPlayer() { return; }
}
