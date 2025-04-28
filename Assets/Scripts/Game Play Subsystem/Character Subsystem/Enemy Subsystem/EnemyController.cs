using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using CharacterEnums;
using WeaponEnums;
using UnityEngine;

public abstract class EnemyController : CharacterBase, IAsyncLoadObject
{
    public GameObject DetectedPlayer {get; set;}
    public Transform ChasingTarget {get; set;}

    // Common variables
    protected Rigidbody2D enemyRigid;


    // Damage & Weapon Info
    protected DamageInfo defaultDamageInfo;
    protected WEAPON_TYPE weaponType;
    protected Queue<WeaponBase> weapons;
    protected List<GameObject> aimingDots;
    protected Vector3 weaponOffset;
    protected bool useShortRangeWeapon;
    protected int weaponCount;
    protected int aimingDotsCount;


    // Terrain checking params
    protected float groundCheckingDistance;
    protected Vector3 groundCheckingVector;
    protected float ObstacleCheckingDistance;
    protected bool checkTerrain;

    // Detecting player params
    protected Vector2 detectRange;
    protected Vector2 rangeOffset;
    protected float attackRange;

    private EnemyStateBase currentState;
    private Dictionary<EnemyState, EnemyStateBase> enemyStateDictionary;
    private TerrainChecker terrainChecker;
    private PlayerDetector playerDetector;
    private DamageArea defalutDamageArea;
    private bool isLoaded = false;



    /****** Initailizing Enemy Character ******/

    // Awake ememy character. Activated on Awake()
    protected override void Awake() 
    { 
        base.Awake();

        enemyRigid = GetComponent<Rigidbody2D>();
    
        InitializeTerrainChecker();
        InitializePlayerDetector();
        InitializeDamageAndWeapon();
        
        StartCoroutine(AsyncEnemyInitialze());
    }

    // Start enemy character. Activated on Start().
    protected override void Start() 
    {
        base.Start();

        StartEnemy();
    }
    protected virtual void StartEnemy(){ }

    private void OnEnable() 
    {
        if( isLoaded )
        {
            // Start current state when enabled
            currentState.OnEnter();
        }
    }

    IEnumerator AsyncEnemyInitialze()
    {
        yield return SetStateDictionary();
        if(checkTerrain) yield return SetTerrainChecker();
        yield return SetPlayerDetector();
        yield return LoadWeaponsAndDots();
        SetDamageArea();
        
        isLoaded = true;
    }

    // Get enemyState prefab and set state components 
    IEnumerator SetStateDictionary()
    {
        enemyStateDictionary = new Dictionary<EnemyState, EnemyStateBase>();

        AsyncOperationHandle<GameObject> enemyState = Addressables.InstantiateAsync("Enemy State", transform);
        yield return enemyState;

        if (enemyState.Status == AsyncOperationStatus.Succeeded)
        {
            Transform enemyStateTrans = enemyState.Result.transform;
            foreach (var state in enemyStateTrans.GetComponents<EnemyStateBase>())
            {
                if (!enemyStateDictionary.ContainsKey(state.GetState())) 
                    enemyStateDictionary[state.GetState()] = state;
            }
            currentState = enemyStateDictionary[EnemyState.Patrolling];
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

    IEnumerator LoadWeaponsAndDots()
    {
        if ( WEAPON_TYPE.WEAPON_TYPE_COUNT != weaponType )
            yield return WeaponFactory.Instance.PoolWeapons(this, weaponType, weapons, weaponCount, useShortRangeWeapon);

        if ( 0 < aimingDotsCount )
            yield return WeaponFactory.Instance.PoolAimingDots(weaponType, aimingDots, aimingDotsCount);
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

        if( DetectedPlayer = playerDetector.DetectPlayer() )
            currentState.DetectedPlayer();

        currentState.OnUpdate();

        UpdateEnemy(); 
    }
    protected virtual void UpdateEnemy() { }

    // Check if enemy can go ahead
    protected bool CanMoveAhead() { return terrainChecker.CanMoveAhead(); }

    public void ChangeState(EnemyState state)
    {
        currentState.OnExit(state);
        currentState = enemyStateDictionary[state];
        currentState.OnEnter();
    }

    public void SetDefaultDamageArea(bool value)
    {
        defalutDamageArea.gameObject.SetActive(value);
    }

    public override void OnDamaged(DamageInfo damageInfo)
    {
        RecentDamagedInfo = damageInfo;
        HitPoint -= RecentDamagedInfo.damageValue;

        currentState.OnDamaged();
    }

    public bool CheckPlayerEnemyDistance()
    {
        if ( null == DetectedPlayer )
            return false;
            
        return (transform.position - DetectedPlayer.transform.position).magnitude < attackRange;
    }

    /****** Abstract Functions ******/
    public abstract void SetPatrolInfo();
    public abstract void Patrol();
    public abstract void ChasePlayer();
    public abstract void SetAttackInfo();
    public abstract bool Attack(); // return if attack is over
    protected abstract void InitializeTerrainChecker();
    protected abstract void InitializePlayerDetector();
    protected abstract void InitializeDamageAndWeapon();
    

}