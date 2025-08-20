using UnityEngine;

public class DamagedState : EnemyStateBase
{
    private const float _FlickerTime = 0.1f;
    private const float _ColorFilter = 0.7f; // Must be in range of (0, 1)
    private Color initialColor;
    private float flickeredTime;

    protected override void Awake()
    {
        base.Awake();

        initialColor = enemySprite.color;
        flickeredTime = 0;
    }

    public override EnemyState GetState() { return EnemyState.Damaged; }

    public override void OnEnter()
    {
        enemySprite.color = initialColor * _ColorFilter;
        flickeredTime = 0;
    }

    public override void OnUpdate()
    {
        flickeredTime += Time.deltaTime;
        if( _FlickerTime < flickeredTime )
            enemyController.ChangeState(EnemyState.Chasing);
    }

    public override void OnExit(EnemyState _)
    {
        enemySprite.color = initialColor;
    }

    public override void DetectedPlayer() { return; }
    public override void OnDamaged() { return; }
}
