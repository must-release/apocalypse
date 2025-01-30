using CharacterEums;
using UnityEngine;

public class ChasingState : EnemyStateBase
{
    private const float FORGET_TIME = 10f;
    private float forgettingTime;

    protected override void StartEnemyState()
    {
        forgettingTime = 0;
    }

    public override ENEMY_STATE GetState() { return ENEMY_STATE.CHASING; }

    public override void OnEnter()
    {
        if ( enemyController.DetectedPlayer )
            enemyController.ChasingTarget = enemyController.DetectedPlayer.transform;
        else if ( null != enemyController.RecentDamagedInfo )
            enemyController.ChasingTarget = enemyController.RecentDamagedInfo.attacker.transform;
        
        if ( null == enemyController.ChasingTarget )
            Debug.LogError("There's nobody to chase!");

        forgettingTime = 0;
    }

    public override void OnUpdate()
    {
        // Chase detected player
        enemyController.ChasePlayer();

        if ( enemyController.CheckPlayerEnemyDistance() )
        {
            enemyController.ChangeState(ENEMY_STATE.ATTACKING); 
            return;
        }

        // Forget player when player is not detected for a while
        if ( null == enemyController.DetectedPlayer )
        {
            forgettingTime += Time.deltaTime;
            if (forgettingTime > FORGET_TIME)
                enemyController.ChangeState(ENEMY_STATE.PATROLLING);
        }
        else
        {
            forgettingTime = 0;
        }
    }

    public override void OnExit(ENEMY_STATE nextState)
    {
        if ( ENEMY_STATE.ATTACKING != nextState)
            enemyController.ChasingTarget = null;
    }

    public override void DetectedPlayer() { return; }
}
