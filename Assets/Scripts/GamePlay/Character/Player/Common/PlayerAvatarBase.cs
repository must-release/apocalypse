using System.Collections.Generic;
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

    public void InitializeAvatar(IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo)
    {
        Debug.Assert(null != objectInteractor, "Object interactor is null");
        Debug.Assert(null != playerMotion, "Player motion is null");
        Debug.Assert(null != playerInfo, "Player info is null");

        _objectInteractor   = objectInteractor;
        _playerMotion       = playerMotion;
        _playerInfo         = playerInfo;

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
    }

    public void OnDamaged(DamageInfo damageInfo)
    {
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

        Logger.Write(LogCategory.GamePlay, $"{LowerState.CurrentState} -> {state}");

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

    [SerializeField] private Animator _lowerAnimator;
    [SerializeField] private Animator _upperAnimator;

    private Dictionary<LowerStateType, IPlayerLowerState> _lowerStateTable = new();
    private Dictionary<UpperStateType, IPlayerUpperState> _upperStateTable = new();

    private IObjectInteractor   _objectInteractor;
    private IMotionController   _playerMotion;
    private ICharacterInfo      _playerInfo;
    private PlayerWeaponBase    _weapon;

    private void Awake()
    {
        Debug.Assert(null != _lowerAnimator, $"Lower Animator is not assigned for {CurrentAvatar}.");
        Debug.Assert(null != _upperAnimator, $"Upper Animator is not assigned for {CurrentAvatar}.");
        _weapon = GetComponent<PlayerWeaponBase>();
        Debug.Assert(null != _weapon, $"Weapon is not assigned for {CurrentAvatar}.");
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
            lower.InitializeState(CurrentAvatar, this, _objectInteractor, _playerMotion, _playerInfo, _lowerAnimator, _weapon);
            LowerStateType state = lower.CurrentState;

            Debug.Assert(false == _lowerStateTable.ContainsKey(state), $"LowerState {state} already registered for {CurrentAvatar}");
            _lowerStateTable.Add(state, lower);
        }

        var uppers = GetComponentsInChildren<IPlayerUpperState>();
        Debug.Assert(0 < uppers.Length, $"No UpperState components found in children for {CurrentAvatar}");
        foreach (var upper in uppers)
        {
            upper.InitializeState(CurrentAvatar, this, _objectInteractor, _playerMotion, _playerInfo, _upperAnimator, _weapon);
            UpperStateType state = upper.CurrentState;

            Debug.Assert(false == _upperStateTable.ContainsKey(state), $"UpperState {state} already registered for {CurrentAvatar}");
            _upperStateTable.Add(state, upper);
        }
    }
}
