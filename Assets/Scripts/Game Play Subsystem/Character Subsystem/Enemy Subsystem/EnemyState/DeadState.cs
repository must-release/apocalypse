using CharacterEums;
using UnityEngine;

public class DeadState : MonoBehaviour, IEnemyState
{
    private Transform enemyTransform;
    private EnemyController enemyController;
    private SpriteRenderer enemySprite;
    private Rigidbody2D enemyRigid;
    private Color enemyColor;
    private const float FADE_OUT_TIME = 1f;

    public ENEMY_STATE GetState() { return ENEMY_STATE.DEAD; }

    public void Start()
    {
        enemyTransform  = transform.parent;
        enemyController = enemyTransform.GetComponent<EnemyController>();
        enemySprite     = enemyTransform.GetComponent<SpriteRenderer>();
        enemyRigid      = enemyTransform.GetComponent<Rigidbody2D>();
    }

    public void StartState()
    {
        enemyColor = enemySprite.color;
        enemyRigid.velocity = Vector2.zero;
        enemyController.SetDefaultDamageArea(false);
    }

    public void UpdateState()
    {
        // Fade out when dead
        enemyColor.a -= Time.deltaTime / FADE_OUT_TIME;
        enemySprite.color = enemyColor;
        if ( enemyColor.a <= 0 )
            enemyController.gameObject.SetActive(false);
    }

    public void EndState(ENEMY_STATE _)
    {
        enemyController.SetDefaultDamageArea(true);
    }

    public void DetectedPlayer() { return; }
    public void OnDamaged() { return; }
}
