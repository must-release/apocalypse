using CharacterEums;
using UnityEngine;

public class DeadState : MonoBehaviour, IEnemyState
{
    private Transform enemyTransform;
    private EnemyController enemyController;
    private SpriteRenderer enemySprite;
    private Color enemyColor;
    private const float FADE_OUT_TIME = 1f;

    public ENEMY_STATE GetState() { return ENEMY_STATE.DEAD; }

    public void Start()
    {
        enemyTransform  = transform.parent;
        enemyController = enemyTransform.GetComponent<EnemyController>();
        enemySprite     = enemyTransform.GetComponent<SpriteRenderer>();
    }

    public void StartState()
    {
        enemyColor = enemySprite.color;
    }

    public void UpdateState()
    {
        // Fade out when dead
        enemyColor.a -= Time.deltaTime / FADE_OUT_TIME;
        enemySprite.color = enemyColor;
        if ( enemyColor.a <= 0 )
        {
            enemyController.gameObject.SetActive(false);
            enemyController.SetDefaultDamageArea(false);
        }
    }

    public void EndState()
    {
        enemyController.SetDefaultDamageArea(true);
    }

    public void DetectedPlayer() { return; }
    public void Attack() { return; }
    public void OnDamaged() { return; }
}
