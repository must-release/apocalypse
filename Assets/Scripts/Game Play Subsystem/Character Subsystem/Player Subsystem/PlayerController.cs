using System.Collections;
using System.Collections.Generic;
using CharacterEums;
using UnityEngine;

public class PlayerController : CharacterBase, IAsyncLoadObject
{
    /****** Public Members ******/

    public float MovingSpeed { get; private set; } = 15f;
    public float JumpingSpeed { get; private set; } = 30f;
    public float Gravity { get; private set; } = 10f;

    public IPlayer CurrentPlayer { get; private set; }
    public PLAYER CurrentCharacter { get; private set; }
    public ControlInfo CurrentControlInfo {get; private set; }
    public PlayerLowerStateBase LowerState { get; private set; }
    public PlayerUpperStateBase UpperState { get; private set; }
    public Animator LowerAnimator { get { return _lowerAnimator; } }
    public Animator UpperAnimator { get { return _upperAnimator; } }


    public bool IsLoaded()
    {
        return _playerDictionary[PLAYER.HEROINE].IsLoaded; // && playerDictionary[CHARACTER.HEROINE].IsLoaded;
    }

    // Set player according to the info
    public void SetPlayer(PLAYER character)
    {
        // Initially change character
        ChangeCharacter(character);

        // Set initial state
        LowerState = _lowerStateDictionary[PLAYER_LOWER_STATE.IDLE];
        UpperState = _upperStateDictionary[PLAYER_UPPER_STATE.IDLE];

        _isReady = true;
    }

    // Control player according to the control info
    public override void ControlCharacter(ControlInfo controlInfo)
    {
        // Set control info of current frame
        CurrentControlInfo = controlInfo; 

        // First, control interactable obejcts. Next, control lower body. Lastly, control upper body.
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
    public void ChangeCharacter(PLAYER character)
    {
        CurrentPlayer.ShowCharacter(false);

        CurrentPlayer = _playerDictionary[character];
        CurrentCharacter = character;

        CurrentPlayer.GetSpriteRenderers( out _lowerSpriteRenderer, out _upperSpriteRenderer );
        CurrentPlayer.GetAnimators( out _lowerAnimator, out _upperAnimator );
        CurrentPlayer.ShowCharacter(true);
    }

    // Change player's lower body state
    public void ChangeLowerState(PLAYER_LOWER_STATE state)
    {
        Debug.Log("Lower : " + LowerState.GetState().ToString() + " -> " + state.ToString());
        LowerState.OnExit();
        LowerState = _lowerStateDictionary[state];
        LowerState.OnEnter();
    }

    // Change player's upper body state
    public void ChangeUpperState(PLAYER_UPPER_STATE state)
    {
        Debug.Log("Uppper : " + UpperState.GetState().ToString() + " -> " + state.ToString());
        UpperState.OnExit(state);
        UpperState = _upperStateDictionary[state];
        UpperState.OnEnter();
    }

    public void AddLowerState(PLAYER_LOWER_STATE stateKey, PlayerLowerStateBase state)
    {
        if (!_lowerStateDictionary.ContainsKey(stateKey)) _lowerStateDictionary[stateKey] = state;
    }

    public void AddUpperState(PLAYER_UPPER_STATE stateKey, PlayerUpperStateBase state)
    {
        if (!_upperStateDictionary.ContainsKey(stateKey)) _upperStateDictionary[stateKey] = state;
    }


    /****** Protected Members ******/

    protected override void Awake()
    {
        base.Awake();

        CharacterHeight = GetComponent<CapsuleCollider2D>().size.y * transform.localScale.y;
        
        _lowerStateDictionary   = new Dictionary<PLAYER_LOWER_STATE, PlayerLowerStateBase>();
        _upperStateDictionary   = new Dictionary<PLAYER_UPPER_STATE, PlayerUpperStateBase>();
        _playerDictionary       = new Dictionary<PLAYER, IPlayer>();
        _playerRigid            = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();

        HitPoint                    = _MaxHitPoint;
        _playerRigid.gravityScale   = Gravity;
        _isReady                    = false;
        _isDamageImmune             = false;
        _flickeringSpeed            = 0.5f; 
        _flickeringCount            = 3;

        _playerDictionary[PLAYER.HERO]      = transform.Find("Hero").GetComponent<IPlayer>();
        _playerDictionary[PLAYER.HEROINE]   = transform.Find("Heroine").GetComponent<IPlayer>();
        CurrentPlayer                       = _playerDictionary[PLAYER.HEROINE];
        CurrentCharacter                    = PLAYER.HEROINE;
        _damagedColor                       = Color.red;
        _originalColor                      = Color.white;
    }


    /****** Private Members ******/
    private const int _MaxHitPoint = 3;

    private Dictionary<PLAYER, IPlayer> _playerDictionary;
    private Dictionary<PLAYER_LOWER_STATE, PlayerLowerStateBase> _lowerStateDictionary;
    private Dictionary<PLAYER_UPPER_STATE, PlayerUpperStateBase> _upperStateDictionary;
    private Rigidbody2D _playerRigid;
    private SpriteRenderer _lowerSpriteRenderer;
    private SpriteRenderer _upperSpriteRenderer;
    private Animator _lowerAnimator;
    private Animator _upperAnimator;
    private Color _damagedColor;
    private Color _originalColor;
    private bool _isReady;
    private bool _isDamageImmune;
    private float _flickeringSpeed;
    private int _flickeringCount;

    private void Update()
    {
        if(!_isReady) return;

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
        }
        else
        {
            UpperState.Enable();
            if (controlInfo.move != 0) UpperState.Move();
            else if (controlInfo.stop) UpperState.Stop();
            if (controlInfo.jump) UpperState.Jump();
            UpperState.Aim(controlInfo.aim);
            if (controlInfo.upDown > 0) UpperState.LookUp(true);
            else UpperState.LookUp(false);
            if (controlInfo.attack) UpperState.Attack();
        }
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


public interface IPlayer
{
    public bool IsLoaded {get; set;}
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