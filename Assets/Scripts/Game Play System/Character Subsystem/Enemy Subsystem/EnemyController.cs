using UnityEngine;

public abstract class EnemyController : CharacterBase
{
    public GameObject DetectedPlayer { get; set; }
    public bool IsPlayerInAttackRange { get; private set; }

    protected Vector2 detectRange;
    protected Vector2 rangeOffset;
    protected float attackRange = 1;

    // Initialize enemy character
    private void Awake()
    {
        InitializeEnemy();
        SetDetectRange();
    }
    public virtual void InitializeEnemy()
    {
        detectRange = new Vector2(20, 8);
        rangeOffset = new Vector2(4, 0);
    }

    // Update enemy character every frame
    private void Update() { UpdateEnemy(); }
    public virtual void UpdateEnemy()
    {
        if (DetectedPlayer) CheckPlayerEnemyDistance();
    }

    // Set default detect range
    private void SetDetectRange()
    {
        CircleCollider2D rangeCollider = gameObject.AddComponent<CircleCollider2D>();
        rangeCollider.radius = detectRange.x / 2; // Set radius based on the original detectRange width
        rangeCollider.offset = rangeOffset;
        rangeCollider.isTrigger = true;
    }

    // Check distance between player and enemy
    private void CheckPlayerEnemyDistance()
    {
        if ((transform.position - DetectedPlayer.transform.position).magnitude < attackRange)
            IsPlayerInAttackRange = true;
        else
            IsPlayerInAttackRange = false;
    }

    /***** Detect Player *****/
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DetectedPlayer = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject == DetectedPlayer)
                DetectedPlayer = null;
            else
                Debug.LogError("Unknown Player Exiting from Detect Range");
        }
    }
}
