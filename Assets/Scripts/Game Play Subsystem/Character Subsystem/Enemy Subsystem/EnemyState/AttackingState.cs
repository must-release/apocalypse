using CharacterEums;
using UnityEngine;

public class AttackingState : MonoBehaviour, IEnemyState
{
    private Transform enemyTransform;
    private EnemyController enemyController;

    public ENEMY_STATE GetState() { return ENEMY_STATE.ATTACKING; }

    public void Start()
    {
        enemyTransform  = transform.parent;
        enemyController = enemyTransform.GetComponent<EnemyController>();
    }

    public void StartState()
    {
        enemyController.SetAttackInfo();
    }

    public void UpdateState()
    {
        if( enemyController.Attack() )
            enemyController.ChangeState(ENEMY_STATE.CHASING);
    }

    public void EndState(ENEMY_STATE _)
    {

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
