using UnityEngine;

public class DeadState : EnemyStateBase
{
    private Color enemyColor;
    private const float FADE_OUT_TIME = 1f;

    protected override void StartEnemyState()
    {
        enemyColor = Color.white;
    }

    public override EnemyState GetState() { return EnemyState.Dead; }

    public override void OnEnter()
    {
        enemyColor = enemySprite.color;
        enemyRigid.linearVelocity = Vector2.zero;
    
        enemyController.SetDefaultDamageArea(false);
    }

    public override void OnUpdate()
    {
        // Fade out when dead
        enemyColor.a -= Time.deltaTime / FADE_OUT_TIME;
        enemySprite.color = enemyColor;
        if ( enemyColor.a <= 0 )
            enemyController.gameObject.SetActive(false);
    }

    public override void OnExit(EnemyState _)
    {
        enemyController.SetDefaultDamageArea(true);
    }

    public override void DetectedPlayer() { return; }
    public override void OnDamaged() { return; }
}
