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


    // Terrain checking params
    protected float groundCheckingDistance;
    protected Vector3 groundCheckingVector;
    protected float ObstacleCheckingDistance;
    protected bool checkTerrain;

    // Detecting player params
    protected Vector2 detectRange;
    protected Vector2 rangeOffset;
    protected float attackRange = 1;

    private IEnemyState currentState;
    private Dictionary<ENEMY_STATE, IEnemyState> enemyStateDictionary;
    private TerrainChecker terrainChecker;
    private bool isLoaded = false;



    /****** Initailizing Enemy Character ******/

    // Awake ememy character. Activated on Awake()
    protected override void AwakeCharacter() 
    { 
        base.AwakeCharacter();
    
        AwakeEnemy();
        StartCoroutine(AsyncEnemyInitialze());
        //SetDetectRange();
    }
    protected virtual void AwakeEnemy()
    {
        // Terrain checker settings
        groundCheckingDistance = 5f;
        groundCheckingVector = new Vector3(1, - 1, 0);
        ObstacleCheckingDistance = 3f;
        checkTerrain = true;


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

    private void OnEnable() 
    {
        if(isLoaded)
        {
            // Start current state when enabled
            currentState.StartState();
        }
    }

    IEnumerator AsyncEnemyInitialze()
    {
        yield return SetStateDictionary();
        if(checkTerrain) yield return SetTerrainChecker();
        
        isLoaded = true;
    }

    // Get enemyState prefab and set state components 
    IEnumerator SetStateDictionary()
    {
        enemyStateDictionary = new Dictionary<ENEMY_STATE, IEnemyState>();

        AsyncOperationHandle<GameObject> enemyState = Addressables.InstantiateAsync("Enemy State", transform);
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
            Debug.LogError("Failed to load the enemy state");
        }
    }

    // Get ray checker prefab and set ray checking settings
    IEnumerator SetTerrainChecker()
    {
        AsyncOperationHandle<GameObject> checker = Addressables.InstantiateAsync("Terrain Checker", transform);
        yield return checker;

        if(checker.Status == AsyncOperationStatus.Succeeded)
        {
            terrainChecker = checker.Result.GetComponent<TerrainChecker>();
            terrainChecker.SetTerrainChecker(groundCheckingDistance, groundCheckingVector,
                 ObstacleCheckingDistance);
        }
        else
        {
            Debug.LogError("Failed to load the Terrain Checker");
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

    // Initialize patrol info
    public abstract void SetPatrolInfo();
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

    // Check if enemy can go ahead
    protected bool CanMoveAhead() { return terrainChecker.CanMoveAhead(); }



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