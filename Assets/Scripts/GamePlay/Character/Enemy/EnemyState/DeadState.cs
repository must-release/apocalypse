using UnityEngine;

public class DeadState : EnemyStateBase
{
    private Color enemyColor;
    private const float FADE_OUT_TIME = 2f;

    protected override void Awake()
    {
        base.Awake();

        enemyColor = Color.white;
    }

    public override EnemyState GetState() { return EnemyState.Dead; }

    public override void OnEnter()
    {
        enemyColor = enemySprite.color;
        enemyRigid.linearVelocity = Vector2.zero;

        enemyController.SetDefaultDamageArea(false);

        _time = FADE_OUT_TIME;
    }

    public override void OnUpdate()
    {
        // Fade out when dead
        _time -= Time.deltaTime / FADE_OUT_TIME;
        if (_time <= 0)
            enemyController.gameObject.SetActive(false);
    }

    public override void OnExit(EnemyState _)
    {
        enemyController.SetDefaultDamageArea(true);
    }

    public override void DetectedPlayer() { return; }
    public override void OnDamaged() { return; }


    private float _time;
}
