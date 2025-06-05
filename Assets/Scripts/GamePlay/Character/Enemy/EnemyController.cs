using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

public abstract class EnemyController : CharacterBase, IAsyncLoadObject
{
    /****** Public Members ******/

    public GameObject   DetectedPlayer  { get; set; }
    public Transform    ChasingTarget   { get; set; }
    public override bool IsPlayer => false;
    public virtual bool IsPlayerInAttackRange
    {
        get
        {
            if (null == DetectedPlayer)
                return false;

            return (transform.position - DetectedPlayer.transform.position).magnitude < attackRange;
        }
    }
    public bool IsLoaded => _isLoaded;

    public void ChangeState(EnemyState state)
    {
        _currentState.OnExit(state);
        _currentState = enemyStateDictionary[state];
        _currentState.OnEnter();
    }

    public void SetDefaultDamageArea(bool value)
    {
        _defalutDamageArea.gameObject.SetActive(value);
    }

    public override void OnDamaged(DamageInfo damageInfo)
    {
        RecentDamagedInfo = damageInfo;
        CurrentHitPoint -= RecentDamagedInfo.damageValue;

        _currentState.OnDamaged();
    }

    public abstract void StartPatrol();
    public abstract void Patrol();
    public abstract void StartChasing();
    public abstract void Chase();
    public abstract void StartAttack();
    public abstract bool Attack(); // return if attack is over


    /****** Protected Members ******/

    protected bool CanMoveAhead => _terrainChecker.CanMoveAhead();
    protected WeaponPoolHandler WeaponPool
    {
        get
        {
            Assert.IsTrue(null != _weaponPoolHandler, $"{weaponType} weapon is not loaded");
            return _weaponPoolHandler;
        }
    }

    // Common variables
    protected Rigidbody2D enemyRigid;


    // Damage & Weapon Info
    protected DamageInfo defaultDamageInfo;
    protected WeaponType weaponType;
    protected Vector3 weaponOffset;
    protected bool useShortRangeWeapon;


    // Terrain checking params
    protected float groundCheckingDistance;
    protected Vector3 groundCheckingVector;
    protected float ObstacleCheckingDistance;
    protected bool checkTerrain;

    // Detecting player params
    protected Vector2 detectRange;
    protected Vector2 rangeOffset;
    protected float attackRange;


    // Awake ememy character. Activated on Awake()
    protected override void Awake() 
    { 
        base.Awake();

        enemyRigid      = GetComponent<Rigidbody2D>();
        _enemyCollider  = GetComponent<Collider2D>();
    
        InitializeTerrainChecker();
        InitializePlayerDetector();
        InitializeDamageAndWeapon();

        AsyncEnemyInitialze().Forget();
    }

    protected virtual void Update()
    {
        // Wait for async loading
        if (!_isLoaded) return;

        if (DetectedPlayer = _playerDetector.DetectPlayer())
            _currentState.DetectedPlayer();

        _currentState.OnUpdate();
    }

    protected void ChangeColliderToTriggerMode(bool value)
    {
        _enemyCollider.isTrigger = value;
    }

    protected abstract void InitializeTerrainChecker();
    protected abstract void InitializePlayerDetector();
    protected abstract void InitializeDamageAndWeapon();


    /****** Private Memvers ******/

    private Dictionary<EnemyState, EnemyStateBase> enemyStateDictionary;

    private EnemyStateBase      _currentState;
    private WeaponPoolHandler   _weaponPoolHandler;
    private TerrainChecker      _terrainChecker;
    private PlayerDetector      _playerDetector;
    private DamageArea          _defalutDamageArea;
    private Collider2D          _enemyCollider;

    private bool _isLoaded;

    private void OnEnable() 
    {
        if( _isLoaded )
        {
            // Start current state when enabled
            _currentState.OnEnter();
        }
    }

    private async UniTask AsyncEnemyInitialze()
    {
        await SetStateDictionary().ToUniTask();
        if (checkTerrain) await SetTerrainChecker().ToUniTask();
        await SetPlayerDetector().ToUniTask();
        await LoadWeapons();
        SetDamageArea();
        
        _isLoaded = true;
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
            _currentState = enemyStateDictionary[EnemyState.Patrolling];
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
            _terrainChecker = checker.Result.GetComponent<TerrainChecker>();
            _terrainChecker.SetTerrainChecker(groundCheckingDistance, groundCheckingVector,
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
            _playerDetector = detector.Result.GetComponent<PlayerDetector>();
            _playerDetector.SetPlayerDetector(detectRange, rangeOffset);
        }
        else
        {
            Debug.LogError("Failed to load the Player Detector");
        }
    }

    private async UniTask LoadWeapons()
    {
        if ( WeaponType.WeaponTypeCount != weaponType )
            _weaponPoolHandler = await WeaponFactory.Instance.AsyncLoadWeaponPoolHandler(weaponType);
    }

    // Create default damage area child object
    private void SetDamageArea()
    {
        GameObject dmgAreaObj = new GameObject("Default Damage Area");
        dmgAreaObj.transform.SetParent(transform, false);
        _defalutDamageArea = dmgAreaObj.AddComponent<DamageArea>();
        _defalutDamageArea.SetDamageArea(transform.GetComponent<Collider2D>(), defaultDamageInfo, true);
    }
}