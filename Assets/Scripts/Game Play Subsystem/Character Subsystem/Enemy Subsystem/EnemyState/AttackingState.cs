using CharacterEums;
using UnityEngine;

public class AttackingState : EnemyStateBase
{

    protected override void StartEnemyState()
    {

    }

    public override ENEMY_STATE GetState() { return ENEMY_STATE.ATTACKING; }

    public override void OnEnter()
    {
        enemyController.SetAttackInfo();
    }

    public override void OnUpdate()
    {
        if( enemyController.Attack() )
            enemyController.ChangeState(ENEMY_STATE.CHASING);
    }

    public override void OnExit(ENEMY_STATE _)
    {

    }

    public override void DetectedPlayer() { return; }
}
