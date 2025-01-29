using CharacterEums;
using JetBrains.Rider.Unity.Editor;
using UnityEngine;

public class DamagedState : MonoBehaviour, IEnemyState
{
    private Transform enemyTransform;
    private EnemyController enemyController;
    private SpriteRenderer enemySprite;
    private const float FLICKER_TIME = 0.1f;
    private const float COLOR_FILTER = 0.7f; // Must be in range of (0, 1)
    private Color initialColor;
    private float flickeredTime;

    public ENEMY_STATE GetState() { return ENEMY_STATE.DAMAGED; }

    public void Start()
    {
        enemyTransform  =   transform.parent;
        enemyController =   enemyTransform.GetComponent<EnemyController>();
        enemySprite     =   enemyTransform.GetComponent<SpriteRenderer>();
        initialColor    =   enemySprite.color;
        flickeredTime   =   0;
    }

    public void StartState()
    {
        enemySprite.color = initialColor * COLOR_FILTER;
        flickeredTime = 0;
    }

    public void UpdateState()
    {
        flickeredTime += Time.deltaTime;
        if(flickeredTime > FLICKER_TIME)
            enemyController.ChangeState(ENEMY_STATE.CHASING);
    }

    public void EndState(ENEMY_STATE _)
    {
        enemySprite.color = initialColor;
    }

    public void DetectedPlayer() { return; }
    public void OnDamaged() { return; }
}
