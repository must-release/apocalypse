using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using CharacterEums;
using UnityEngine;

public abstract class EnemyController : CharacterBase, SceneObejct
{

    public GameObject DetectedPlayer {get; set;}
    public Transform ChasingTarget {get; set;}

    // Damage Info
    protected DamageInfo defaultDamageInfo;
    protected DamageInfo attackDamageInfo;


    // Terrain checking params
    protected float groundCheckingDistance;
    protected Vector3 groundCheckingVector;
    protected float ObstacleCheckingDistance;
    protected bool checkTerrain;

    // Detecting player params
    protected Vector2 detectRange;
    protected Vector2 rangeOffset;
    protected float attackRange;

    private IEnemyState currentState;
    private Dictionary<ENEMY_STATE, IEnemyState> enemyStateDictionary;
    private TerrainChecker terrainChecker;
    private PlayerDetector playerDetector;
    private DamageArea defalutDamageArea;
    private bool isLoaded = false;



    /****** Initailizing Enemy Character ******/

    // Awake ememy character. Activated on Awake()
    protected override void AwakeCharacter() 
    { 
        base.AwakeCharacter();
    
        AwakeEnemy();
        StartCoroutine(AsyncEnemyInitialze());
    }
    protected virtual void AwakeEnemy()
    {
        // Terrain checker settings
        groundCheckingDistance = 5f;
        groundCheckingVector = new Vector3(1, - 1, 0);
        ObstacleCheckingDistance = 3f;
        checkTerrain = true;

        // Player detector settings
        detectRange = new Vector2(20, 8);
        rangeOffset = new Vector2(4, 0);

        // Set damage Info
        defaultDamageInfo = new DamageInfo();
        defaultDamageInfo.attacker = gameObject;
        defaultDamageInfo.damageValue = 1;
        attackDamageInfo = new DamageInfo();
        attackDamageInfo.damageValue = 1;

        // Set attack range
        attackRange = 10;
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
        yield return SetPlayerDetector();
        SetDamageArea();
        
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

    // Get terrain checker prefab and set ray checking settings
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

    // Get player detector prefab and set detecting settings
    IEnumerator SetPlayerDetector()
    {
        AsyncOperationHandle<GameObject> detector = Addressables.InstantiateAsync("Player Detector", transform);
        yield return detector;

        if (detector.Status == AsyncOperationStatus.Succeeded)
        {
            playerDetector = detector.Result.GetComponent<PlayerDetector>();
            playerDetector.SetPlayerDetector(detectRange, rangeOffset);
        }
        else
        {
            Debug.LogError("Failed to load the Player Detector");
        }
    }

    // Create default damage area child object
    private void SetDamageArea()
    {
        GameObject dmgAreaObj = new GameObject("Default Damage Area");
        dmgAreaObj.transform.SetParent(transform, false);
        defalutDamageArea = dmgAreaObj.AddComponent<DamageArea>();
        defalutDamageArea.SetDamageArea(transform.GetComponent<Collider2D>(), defaultDamageInfo, true);
    }

    public bool IsLoaded() { return isLoaded; }




    /****** Operate Enemy Character ******/

    // Update enemy character every frame
    private void Update() 
    {   
        // Wait for async loading
        if(!isLoaded) return; 

        // Check if player is detected
        if(DetectedPlayer = playerDetector.DetectPlayer())
        {
            // Player detected
            currentState.DetectedPlayer();

            // Check if enemy should attack
            if(CheckPlayerEnemyDistance())
            {
                currentState.Attack();
            }
        }

        // Update current state
        currentState.UpdateState();

        UpdateEnemy(); 
    }
    protected virtual void UpdateEnemy() { }

    // Check if enemy can go ahead
    protected bool CanMoveAhead() { return terrainChecker.CanMoveAhead(); }

    public void ChangeState(ENEMY_STATE state)
    {
        currentState.EndState();
        currentState = enemyStateDictionary[state];
        currentState.StartState();
    }

    public void SetDefaultDamageArea(bool value)
    {
        defalutDamageArea.gameObject.SetActive(value);
    }

    public override void OnDamaged(DamageInfo damageInfo)
    {
        currentState.OnDamaged();
    }


    // Check distance between player and enemy
    private bool CheckPlayerEnemyDistance()
    {
        return (transform.position - DetectedPlayer.transform.position).magnitude < attackRange;
    }

    /****** Abstract Functions ******/

    // Initialize patrol info
    public abstract void SetPatrolInfo();
    // Circle patrol area 
    public abstract void Patrol();
    // Chase detected player
    public abstract void ChasePlayer();
}

public interface IEnemyState
{
    public ENEMY_STATE GetState();
    public void StartState();
    public void UpdateState();
    public void EndState();
    public void DetectedPlayer();
    public void Attack();
    public void OnDamaged();
}