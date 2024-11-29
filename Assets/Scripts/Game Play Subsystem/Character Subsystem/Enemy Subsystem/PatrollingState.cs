using CharacterEums;
using UnityEngine;

public class PatrollingState : MonoBehaviour, IEnemyState
{
    private Transform enemyTransform;
    private EnemyController enemyController;

    public ENEMY_STATE GetState() { return ENEMY_STATE.PATROLLING; }

    public void Start()
    {
        enemyTransform = transform.parent;
        enemyController = enemyTransform.GetComponent<EnemyController>();
    }

    public void StartState()
    {
        // Set initial info for patrolling
        enemyController.SetPatrolInfo();
    }

    public void UpdateState()
    {
        // Circle patrol area
        enemyController.Patrol();
    }

    public void EndState()
    {

    }

    public void DetectedPlayer()
    {
        enemyController.ChangeState(ENEMY_STATE.CHASING);
    }



    public void Attack() { return; }
}
