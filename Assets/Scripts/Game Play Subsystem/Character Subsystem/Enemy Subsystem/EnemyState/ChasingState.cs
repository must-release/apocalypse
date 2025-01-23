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
        if( enemyController.DetectedPlayer )
            enemyController.ChasingTarget = enemyController.DetectedPlayer.transform;
        else if ( enemyController.RecentDamagedInfo.attacker )
            enemyController.ChasingTarget = enemyController.RecentDamagedInfo.attacker.transform;
        else
            Debug.LogError("There's nobody to chase!");

        forgettingTime = 0;
    }

    public void UpdateState()
    {
        // Chase detected player
        enemyController.ChasePlayer();

        // Forget player when player is no detected for a while
        if (enemyController.DetectedPlayer == null)
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

    public void EndState()
    {
        enemyController.ChasingTarget = null;
    }

    public void OnDamaged() 
    { 
        enemyController.ChangeState(ENEMY_STATE.DAMAGED);
    }

    public void DetectedPlayer() { return; }
    public void Attack() { return; }
}
