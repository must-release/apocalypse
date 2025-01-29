using CharacterEums;
using UnityEngine;

public class ChasingState : MonoBehaviour, IEnemyState
{
    private Transform enemyTransform;
    private EnemyController enemyController;
    private const float FORGET_TIME = 10f;
    private float forgettingTime;

    public ENEMY_STATE GetState() { return ENEMY_STATE.CHASING; }

    public void Start()
    {
        enemyTransform = transform.parent;
        enemyController = enemyTransform.GetComponent<EnemyController>();
    }

    public void StartState()
    {
        if ( enemyController.DetectedPlayer )
            enemyController.ChasingTarget = enemyController.DetectedPlayer.transform;
        else if ( null != enemyController.RecentDamagedInfo )
            enemyController.ChasingTarget = enemyController.RecentDamagedInfo.attacker.transform;
        
        if ( null == enemyController.ChasingTarget )
            Debug.LogError("There's nobody to chase!");

        forgettingTime = 0;
    }

    public void UpdateState()
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

    public void EndState(ENEMY_STATE nextState)
    {
        if ( ENEMY_STATE.ATTACKING != nextState)
            enemyController.ChasingTarget = null;
    }

    public void OnDamaged() 
    { 
        if( 0 < enemyController.HitPoint )
            enemyController.ChangeState(ENEMY_STATE.DAMAGED);
        else
            enemyController.ChangeState(ENEMY_STATE.DEAD);
    }

    public void DetectedPlayer() { return; }
}
