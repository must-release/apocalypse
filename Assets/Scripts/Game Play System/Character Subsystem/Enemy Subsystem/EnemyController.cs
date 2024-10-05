using UnityEngine;

public abstract class EnemyController : CharacterBase
{
    public GameObject DetectedPlayer {get; private set;}
    public bool IsPlayerInAttackRange {get; private set;}

    protected Vector2 detectRange;
    protected Vector2 rangeOffset;
    protected float attackRange = 1;

    // Initialize ememy character
    protected override void InitializeCharacter() 
    { 
        base.InitializeCharacter();
        InitializeEnemy();
        SetDetectRange();
    }
    public virtual void InitializeEnemy()
    {
        detectRange = new Vector2(20,8);
        rangeOffset = new Vector2(4,0);
    }

    // Update enemy character every frame
    private void Update() { UpdateEnemy(); }
    public virtual void UpdateEnemy()
    {
        if(DetectedPlayer) CheckPlayerEnemyDistance(); 
    }

    // Set defalut detect range
    private void SetDetectRange()
    {   
        BoxCollider2D rangeColider = gameObject.AddComponent<BoxCollider2D>();
        rangeColider.size = detectRange;
        rangeColider.offset = rangeOffset;
        rangeColider.isTrigger = true; 
    }

    // Check distance between player and enemy
    private void CheckPlayerEnemyDistance()
    {
        if((transform.position - DetectedPlayer.transform.position).magnitude < attackRange)
            IsPlayerInAttackRange = true;
        else
            IsPlayerInAttackRange = false;
    }



    /***** Detect Player *****/
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            DetectedPlayer = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {        
        if(other.CompareTag("Player"))
        {
            if(other.gameObject == DetectedPlayer)
                DetectedPlayer = null;
            else
                Debug.LogError("Unknown Player Exiting from Detect Range");
        }
    }

    
}


