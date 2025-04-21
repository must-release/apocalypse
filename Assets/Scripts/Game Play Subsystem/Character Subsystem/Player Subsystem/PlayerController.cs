using System.Collections;
using System.Collections.Generic;
using CharacterEums;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : CharacterBase, IAsyncLoadObject
{
    /****** Public Members ******/

    public float MovingSpeed { get; private set; }  = 15f;
    public float JumpingSpeed { get; private set; } = 30f;
    public float Gravity { get; private set; }      = 10f;

    public IPlayerAvatar        CurrentAvatar { get; private set; }     = null;
    public PlayerType           CurrentPlayerType { get; private set; } = PlayerType.PlayerCount;
    public ControlInfo          CurrentControlInfo {get; private set; } = null;
    public PlayerLowerStateBase LowerState { get; private set; }        = null;
    public PlayerUpperStateBase UpperState { get; private set; }        = null;
    public PlayerLowerAnimatorBase LowerAnimator { get; private set; }  = null;
    public PlayerUpperAnimatorBase UpperAnimator { get; private set; }  = null;


    public bool IsLoaded()
    {
        return _avatarDictionary[PlayerType.Heroine].IsLoaded(); // && playerDictionary[CHARACTER.HEROINE].IsLoaded;
    }

    public void InitializePlayer(PlayerType player)
    {
        // Initially change character
        CurrentPlayerType   = player;
        CurrentAvatar       = _avatarDictionary[CurrentPlayerType];

        // Set initial state
        LowerState = _lowerStateDictionary[PlayerLowerState.Idle];
        UpperState = _upperStateDictionary[PlayerUpperState.Idle];

        // set initial animator
        UpperAnimator = _upperAnimatorDictionary[CurrentPlayerType];
        LowerAnimator = _lowerAnimatorDictionary[CurrentPlayerType];


        CurrentAvatar.ShowCharacter(true);

        _isInitilized = true;
    }

    // Control player according to the control info
    public override void ControlCharacter(ControlInfo controlInfo)
    {
        // Set control info of current frame
        CurrentControlInfo = controlInfo; 

        // First, control interactable objects. Next, control lower body. Lastly, control upper body.
        ControlInteractionObjects(controlInfo);
        ControlLowerBody(controlInfo);
        ControlUpperBody(controlInfo);
    }

    // Called once when player is on air
    public override void OnAir()
    { 
        LowerState.OnAir();
        UpperState.OnAir();
    }

    // Called once when player is on ground
    public override void OnGround() 
    { 
        LowerState.OnGround();
        UpperState.OnGround();
    }

    public override void OnDamaged(DamageInfo damageInfo) 
    {
        if ( _isDamageImmune ) return;

        RecentDamagedInfo = damageInfo;
        HitPoint -= RecentDamagedInfo.damageValue;

        StartCoroutine(StartDamageImmuneState());

        LowerState.Damaged(); 
        UpperState.Disable();
    }

    // Change player character
    public void ChangePlayer(PlayerType player)
    {
        CurrentAvatar.ShowCharacter(false);

        CurrentPlayerType   = player;
        CurrentAvatar       = _avatarDictionary[CurrentPlayerType];
        LowerAnimator       = _lowerAnimatorDictionary[CurrentPlayerType];
        UpperAnimator       = _upperAnimatorDictionary[CurrentPlayerType];

        CurrentAvatar.GetSpriteRenderers( out _lowerSpriteRenderer, out _upperSpriteRenderer );
        CurrentAvatar.ShowCharacter(true);
    }

    // Change player's lower body state
    public void ChangeLowerState(PlayerLowerState state)
    {
        Debug.Log("Lower : " + LowerState.GetStateType().ToString() + " -> " + state.ToString());
        LowerState.OnExit();
        LowerState = _lowerStateDictionary[state];
        LowerState.OnEnter();
    }

    // Change player's upper body state
    public void ChangeUpperState(PlayerUpperState state)
    {
        Debug.Log("Upper : " + UpperState.GetStateType().ToString() + " -> " + state.ToString());
        UpperState.OnExit(state);
        UpperState = _upperStateDictionary[state];
        UpperState.OnEnter();
    }

    public void RegisterLowerState(PlayerLowerState stateKey, PlayerLowerStateBase state)
    {
        if (false == _lowerStateDictionary.ContainsKey(stateKey))
        {
            _lowerStateDictionary.Add(stateKey, state);
        }
    }

    public void RegisterUpperState(PlayerUpperState stateKey, PlayerUpperStateBase state)
    {
        if (false == _upperStateDictionary.ContainsKey(stateKey))
        {
            _upperStateDictionary.Add(stateKey, state);
        }
    }

    public void RegisterLowerAnimator(PlayerType playerType, PlayerLowerAnimatorBase animator)
    {
        if (false == _lowerAnimatorDictionary.ContainsKey(playerType))
        {
            _lowerAnimatorDictionary.Add(playerType, animator);
        }    
    }

    public void RegisterUpperAnimator(PlayerType playerType, PlayerUpperAnimatorBase animator)
    {
        if (false == _upperAnimatorDictionary.ContainsKey(playerType))
        {
            _upperAnimatorDictionary.Add(playerType, animator);
        }
    }

    public void RegisterAvatar(PlayerType playerType, IPlayerAvatar avatar)
    {
        if (false == _avatarDictionary.ContainsKey(playerType))
        {
            _avatarDictionary.Add(playerType, avatar);
        }
    }

    /****** Protected Members ******/

    protected override void Awake()
    {
        Assert.IsTrue(null != _heroTransform, "Hero Transform is not assigned in the editor.");
        Assert.IsTrue(null != _heroineTransform, "Heroine Transform is not assigned in the editor.");

        base.Awake();

        CharacterHeight = GetComponent<CapsuleCollider2D>().size.y * transform.localScale.y;
        
        _lowerStateDictionary       = new Dictionary<PlayerLowerState, PlayerLowerStateBase>();
        _upperStateDictionary       = new Dictionary<PlayerUpperState, PlayerUpperStateBase>();
        _lowerAnimatorDictionary    = new Dictionary<PlayerType, PlayerLowerAnimatorBase>();
        _upperAnimatorDictionary    = new Dictionary<PlayerType, PlayerUpperAnimatorBase>();
        _avatarDictionary           = new Dictionary<PlayerType, IPlayerAvatar>();
        _playerAssembler            = new PlayerAssembler(this, _heroTransform, _heroineTransform);
        _playerRigid                = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();

        HitPoint                    = _MaxHitPoint;
        _playerRigid.gravityScale   = Gravity;
        _isInitilized               = false;
        _isDamageImmune             = false;
        _flickeringSpeed            = 0.5f; 
        _flickeringCount            = 3;
        _damagedColor               = Color.red;
        _originalColor              = Color.white;


        _playerAssembler.Assemble();
    }


    /****** Private Members ******/
    private const int _MaxHitPoint = 3;

    [SerializeField] private Transform _heroTransform;
    [SerializeField] private Transform _heroineTransform;

    private Dictionary<PlayerType, IPlayerAvatar> _avatarDictionary; 
    private Dictionary<PlayerType, PlayerLowerAnimatorBase> _lowerAnimatorDictionary;
    private Dictionary<PlayerType, PlayerUpperAnimatorBase> _upperAnimatorDictionary;
    private Dictionary<PlayerLowerState, PlayerLowerStateBase> _lowerStateDictionary;
    private Dictionary<PlayerUpperState, PlayerUpperStateBase> _upperStateDictionary;
    private PlayerAssembler _playerAssembler;
    private Rigidbody2D _playerRigid;
    private SpriteRenderer _lowerSpriteRenderer;
    private SpriteRenderer _upperSpriteRenderer;
    private Color _damagedColor;
    private Color _originalColor;
    private bool _isInitilized;
    private bool _isDamageImmune;
    private float _flickeringSpeed;
    private int _flickeringCount;

    private void Update()
    {
        if(false == _isInitilized) return;

        // Update player's state
        LowerState.OnUpdate();
        UpperState.OnUpdate();
    }

    // Control player's lower body
    private void ControlLowerBody(ControlInfo controlInfo)
    {
        // Change player state according to the input control info
        if (controlInfo.move != 0) LowerState.Move(controlInfo.move);
        else if (controlInfo.stop) LowerState.Stop();
        if (controlInfo.jump) LowerState.Jump();
        if (controlInfo.tag) LowerState.Tag();
        LowerState.Aim(controlInfo.aim != Vector3.zero);
        LowerState.UpDown(controlInfo.upDown);

        // Change player state according to the object control info
        LowerState.Climb(controlInfo.climb);
        LowerState.Push(controlInfo.push);
    }

    // Control player's upper body
    private void ControlUpperBody(ControlInfo controlInfo)
    {   
        if (LowerState.DisableUpperBody())
        {
            UpperState.Disable();
            return;
        }
        
        UpperState.Enable();
        if (controlInfo.move != 0) UpperState.Move();
        else if (controlInfo.stop) UpperState.Stop();
        if (controlInfo.jump) UpperState.Jump();
        UpperState.Aim(controlInfo.aim);
        if (controlInfo.upDown > 0) UpperState.LookUp(true);
        else UpperState.LookUp(false);
        if (controlInfo.attack) UpperState.Attack();
    }

    private IEnumerator StartDamageImmuneState()
    {
        _isDamageImmune = true;

        // Change character's color
        for (int i = 0; i < _flickeringCount; i++)
        {
            yield return StartCoroutine(LerpColor(_originalColor, _damagedColor, _flickeringSpeed));
            yield return StartCoroutine(LerpColor(_damagedColor, _originalColor, _flickeringSpeed));
        }

        _isDamageImmune = false;
    }

    private IEnumerator LerpColor(Color startColor, Color targetColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            _lowerSpriteRenderer.material.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            _upperSpriteRenderer.material.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _lowerSpriteRenderer.material.color = targetColor;
        _upperSpriteRenderer.material.color = targetColor;
    }
}


public interface IPlayerAvatar : IAsyncLoadObject
{
    public Transform GetTransform();
    public void GetAnimators( out Animator lowerAnimator, out Animator upperAnimator );
    public void GetSpriteRenderers( out SpriteRenderer lowerSpriteRenderer, out SpriteRenderer upperSpriteRenderer );
    public IEnumerator LoadWeaponsAndDots();
    public void ShowCharacter(bool value);
    public void RotateUpperBody(float rotateAngle);
    public void RotateUpperBody(Vector3 target);
    public void Aim(bool value);
    public float Attack(); // Execute attack and return attack cool time
}