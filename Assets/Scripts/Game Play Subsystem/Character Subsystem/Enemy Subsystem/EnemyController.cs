using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using CharacterEums;
using UnityEngine;

public abstract class EnemyController : CharacterBase, SceneObejct
{
    public GameObject DetectedPlayer { get; set; }
    public bool IsPlayerInAttackRange { get; private set; }

    protected Vector2 detectRange;
    protected Vector2 rangeOffset;
    protected float attackRange = 1;

    private IEnemyState currentState;
    private Dictionary<ENEMY_STATE, IEnemyState> enemyStateDictionary;
    private bool isLoaded = false;



    /****** Initailizing Enemy Character ******/

    // Awake ememy character. Activated on Awake()
    protected override void AwakeCharacter() 
    { 
        base.AwakeCharacter();
    
        StartCoroutine(AsyncEnemyInitialze());
        AwakeEnemy();
        //SetDetectRange();
    }
    protected virtual void AwakeEnemy()
    {
        detectRange = new Vector2(20, 8);
        rangeOffset = new Vector2(4, 0);
    }

    // Start enemy character. Activated on Start().
    protected override void StartCharacter() 
    {
        base.StartCharacter();

        StartEnemy();
    }
    protected virtual void StartEnemy(){ }

    IEnumerator AsyncEnemyInitialze()
    {
        yield return SetStateDictionary();

        isLoaded = true;
    }

    // Get enemyState prefab and set state components 
    IEnumerator SetStateDictionary()
    {
        enemyStateDictionary = new Dictionary<ENEMY_STATE, IEnemyState>();

        AsyncOperationHandle<GameObject> enemyState = Addressables.InstantiateAsync("Enemy Utilities/EnemyState", transform);
        yield return enemyState;

        if (enemyState.Status == AsyncOperationStatus.Succeeded)
        {
            Transform enemyStateTrans = enemyState.Result.transform;
            foreach (var state in enemyStateTrans.GetComponents<IEnemyState>())
            {
                if (!enemyStateDictionary.ContainsKey(state.GetState())) 
                    enemyStateDictionary[state.GetState()] = state;
            }
            currentState = enemyStateDictionary[ENEMY_STATE.PATROLLING];
        }
        else
        {
            Debug.LogError("Failed to load the enemy state: ");
        }
    }

    public bool IsLoaded() { return isLoaded; }

    /****** Operate Enemy Character ******/

    // Update enemy character every frame
    private void Update() 
    {   
        // Wait for async loading
        if(!isLoaded) return; 

        UpdateEnemy(); 
    }
    protected virtual void UpdateEnemy()
    {
        // Check if enemy should attack
        if (DetectedPlayer) CheckPlayerEnemyDistance();

        // Update current state
        currentState.UpdateState();
    }

    // Circle patrol area 
    public abstract void Patrol();


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


public interface IEnemyState
{
    public ENEMY_STATE GetState();
    public void StartState();
    public void UpdateState();
    public void EndState();
}