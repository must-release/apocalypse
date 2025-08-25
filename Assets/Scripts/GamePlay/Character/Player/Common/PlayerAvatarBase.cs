using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;


public abstract class PlayerAvatarBase : MonoBehaviour, IPlayerAvatar, ILowerStateController, IUpperStateController
{
    /****** Public Members ******/

    public abstract PlayerAvatarType CurrentAvatar { get; }
    public bool IsLoaded
    {
        get
        {
            if (false == _weapon.IsLoaded)
                return false;

            return true;
        }
    }
    public bool IsDamageImmune { get; private set; }

    public void InitializeAvatar(IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo, ControlInputBuffer inputBuffer)
    {
        Debug.Assert(null != objectInteractor, "Object interactor is null");
        Debug.Assert(null != playerMotion, "Player motion is null");
        Debug.Assert(null != playerInfo, "Player info is null");
        Debug.Assert(null != inputBuffer, "Input buffer is null");

        _objectInteractor   = objectInteractor;
        _playerMotion       = playerMotion;
        _playerInfo         = playerInfo;
        _inputBuffer        = inputBuffer;

        RegisterStates();
    }

    public void ControlAvatar(IReadOnlyControlInfo controlInfo)
    {
        Debug.Assert(null != controlInfo, "Control info is null");

        ControlLowerBody(controlInfo);
        ControlUpperBody(controlInfo);
    }

    public void ActivateAvatar(bool value)
    {
        Debug.Assert(_lowerStateTable.ContainsKey(LowerStateType.Idle), "Idle state not found");
        Debug.Assert(_upperStateTable.ContainsKey(UpperStateType.Idle), "Idle state not found");

        if (false == value)
        {
            LowerState?.OnExit(LowerStateType.Idle);
            UpperState?.OnExit(UpperStateType.Idle);
        }

        LowerState = _lowerStateTable[LowerStateType.Idle];
        UpperState = _upperStateTable[UpperStateType.Idle];
        UpperState.LowerStateInfo = LowerState;

        gameObject.SetActive(value);
    }

    public void OnUpdate()
    {
        Debug.Assert(IsLoaded, "Avatar is not loaded yet");

        LowerState.OnUpdate();
        UpperState.OnUpdate();
    }

    public void OnFixedUpdate()
    {
        Debug.Assert(IsLoaded, "Avatar is not loaded yet");

        LowerState.OnFixedUpdate();
        UpperState.OnFixedUpdate();
    }

    public void OnAir()
    {
        LowerState.OnAir();
        UpperState.OnAir();
    }

    public void OnGround()
    {
        LowerState.OnGround();
        UpperState.OnGround();
    }

    public void OnDamaged(DamageInfo damageInfo)
    {
        StartDamageImmuneStateAsync().Forget();

        LowerState.OnDamaged();
        UpperState.Disable();
    }

    public void OnDead()
    {
        ChangeState(LowerStateType.Dead);
        UpperState.Disable();
    }

    public void ChangeState(LowerStateType state)
    {
        Debug.Assert(_lowerStateTable.ContainsKey(state), "Invalid Lower State");

        LowerState.OnExit(state);
        LowerState = _lowerStateTable[state];
        LowerState.OnEnter();

        UpperState.LowerStateInfo = LowerState;

        if (LowerState.ShouldDisableUpperBody)
            UpperState.Disable();
        else
            UpperState.Enable();
    }

    public void ChangeState(UpperStateType state)
    {
        Debug.Assert(_upperStateTable.ContainsKey(state), "Invalid Upper State");

        Logger.Write(LogCategory.GamePlay, $"{UpperState.CurrentState} => {state}");

        UpperState.OnExit(state);
        UpperState = _upperStateTable[state];
        UpperState.LowerStateInfo = LowerState;
        UpperState.OnEnter();
    }


    /****** Protected Members ******/

    protected IPlayerLowerState LowerState { get; private set; }
    protected IPlayerUpperState UpperState { get; private set; }
    protected abstract void ControlLowerBody(IReadOnlyControlInfo controlInfo);
    protected abstract void ControlUpperBody(IReadOnlyControlInfo controlInfo);


    /****** Private Members ******/

    [SerializeField] private Transform _lowerBodyTransfrom;
    [SerializeField] private Transform _upperBodyTransfrom;

    private const float _DamageImmuneTime           = 2f;
    private const float _AlphaColorValueOnDamage    = 0.5f; // Must be between 0 ~ 1.0
    private const int   _FlickerCountOnDamage       = 4;

    private Dictionary<LowerStateType, IPlayerLowerState> _lowerStateTable = new();
    private Dictionary<UpperStateType, IPlayerUpperState> _upperStateTable = new();

    private IObjectInteractor           _objectInteractor;
    private IMotionController           _playerMotion;
    private ICharacterInfo              _playerInfo;
    private ControlInputBuffer          _inputBuffer;
    private PlayerWeaponBase            _weapon;
    private Animator                    _lowerAnimator;
    private SpriteRenderer              _lowerSprite;
    private Animator                    _upperAnimator;
    private SpriteRenderer              _upperSprite;

    private void OnValidate()
    {
        Debug.Assert(null != _lowerBodyTransfrom, $"Lower body transform is not assigned for {CurrentAvatar}.");
        Debug.Assert(null != _lowerBodyTransfrom, $"Upper body transform is not assigned for {CurrentAvatar}.");
        Debug.Assert(null != _lowerBodyTransfrom.GetComponent<Animator>(), $"Lower body does not have Animator component in {CurrentAvatar}.");
        Debug.Assert(null != _lowerBodyTransfrom.GetComponent<SpriteRenderer>(), $"Lower body does not have SpriteRenderer component in {CurrentAvatar}.");
        Debug.Assert(null != _upperBodyTransfrom.GetComponent<Animator>(), $"Upper body does not have Animator component in {CurrentAvatar}.");
        Debug.Assert(null != _upperBodyTransfrom.GetComponent<SpriteRenderer>(), $"Upper body does not have SpriteRenderer component in {CurrentAvatar}.");
        Debug.Assert(null != GetComponent<PlayerWeaponBase>(), $"Weapon is not assigned for {CurrentAvatar}.");
    }

    private void Awake()
    {
        _lowerAnimator  = _lowerBodyTransfrom.GetComponent<Animator>();
        _lowerSprite    = _lowerBodyTransfrom.GetComponent<SpriteRenderer>();
        _upperAnimator  = _upperBodyTransfrom.GetComponent<Animator>();
        _upperSprite    = _upperBodyTransfrom.GetComponent<SpriteRenderer>();
        _weapon         = GetComponent<PlayerWeaponBase>();
    }

    private void OnEnable()
    {
        if (false == IsLoaded) return;

        LowerState.OnEnter();
        UpperState.OnEnter();
    }


    private void RegisterStates()
    {
        Debug.Assert(null != _playerMotion, $"Player Motion is not assigned for {CurrentAvatar}");
        Debug.Assert(null != _playerInfo, $"Player Info is not assigned for {CurrentAvatar}");

        var lowers = GetComponentsInChildren<IPlayerLowerState>();
        Debug.Assert(0 < lowers.Length, $"No LowerState components found in children for {CurrentAvatar}");
        foreach (var lower in lowers)
        {
            lower.InitializeState(CurrentAvatar, this, _objectInteractor, _playerMotion, _playerInfo, _lowerAnimator, _weapon, _inputBuffer);
            LowerStateType state = lower.CurrentState;

            Debug.Assert(false == _lowerStateTable.ContainsKey(state), $"LowerState {state} already registered for {CurrentAvatar}");
            _lowerStateTable.Add(state, lower);
        }

        var uppers = GetComponentsInChildren<IPlayerUpperState>();
        Debug.Assert(0 < uppers.Length, $"No UpperState components found in children for {CurrentAvatar}");
        foreach (var upper in uppers)
        {
            upper.InitializeState(CurrentAvatar, this, _objectInteractor, _playerMotion, _playerInfo, _upperAnimator, _weapon, _inputBuffer);
            UpperStateType state = upper.CurrentState;

            Debug.Assert(false == _upperStateTable.ContainsKey(state), $"UpperState {state} already registered for {CurrentAvatar}");
            _upperStateTable.Add(state, upper);
        }
    }

    private async UniTask StartDamageImmuneStateAsync()
    {
        Debug.Assert(false == IsDamageImmune, $"Already in damage immune state in {CurrentAvatar}");

        IsDamageImmune = true;
        await PlayFlickerEffect();
        IsDamageImmune = false;
    }

    private Tween PlayFlickerEffect()
    {
        Debug.Assert(0 < _FlickerCountOnDamage, $"Flicker count must be greater than 0 in {CurrentAvatar}");

        float flickerDuration = _DamageImmuneTime / (_FlickerCountOnDamage * 2);
        var sequence = DOTween.Sequence().SetLink(gameObject, LinkBehaviour.KillOnDestroy);

        for (int i = 0; i < _FlickerCountOnDamage; ++i)
        {
            sequence.Append(_lowerSprite.DOFade(_AlphaColorValueOnDamage, flickerDuration));
            sequence.Join(_upperSprite.DOFade(_AlphaColorValueOnDamage, flickerDuration));

            sequence.Append(_lowerSprite.DOFade(1.0f, flickerDuration));
            sequence.Join(_upperSprite.DOFade(1.0f, flickerDuration));
        }

        return sequence;
    }
}
