using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public abstract class PlayerAvatarBase : MonoBehaviour, IPlayerAvatar, ILowerStateController, IUpperStateController
{
    /****** Public Members ******/

    public abstract PlayerType CurrentPlayer { get; }
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
        Assert.IsTrue(null != objectInteractor, "Object interactor is null");
        Assert.IsTrue(null != playerMotion, "Player motion is null");
        Assert.IsTrue(null != playerInfo, "Player info is null");

        _objectInteractor   = objectInteractor;
        _playerMotion       = playerMotion;
        _playerInfo         = playerInfo;

        RegisterStates();
    }

    public void ControlAvatar(IReadOnlyControlInfo controlInfo)
    {
        Assert.IsTrue(null != controlInfo, "Control info is null");

        ControlLowerBody(controlInfo);
        ControlUpperBody(controlInfo);
    }    

    public void ActivateAvatar(bool value)
    {
        Assert.IsTrue(_lowerStateTable.ContainsKey(LowerStateType.Idle), "Idle state not found");
        Assert.IsTrue(_upperStateTable.ContainsKey(UpperStateType.Idle), "Idle state not found");

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
        Assert.IsTrue(IsLoaded, "Avatar is not loaded yet");

        LowerState.OnUpdate();
        UpperState.OnUpdate();
    }

    public void OnFixedUpdate()
    {
        Assert.IsTrue(IsLoaded, "Avatar is not loaded yet");

        UpperState.OnFixedUpdate();
        LowerState.OnFixedUpdate();
    }

    public void OnAir()
    {
        LowerState.OnAir();
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
        Assert.IsTrue(_lowerStateTable.ContainsKey(state), "Invalid Lower State");

        Logger.Write(LogCategory.GamePlay, $"{LowerState.StateType} -> {state}");

        LowerState.OnExit(state);
        LowerState = _lowerStateTable[state];
        LowerState.OnEnter();

        UpperState.LowerBodyStateInfo = LowerState;

        if (LowerState.ShouldDisableUpperBody)
            UpperState.Disable();
        else
            UpperState.Enable();
    }

    public void ChangeState(UpperStateType state)
    {
        Assert.IsTrue(_upperStateTable.ContainsKey(state), "Invalid Upper State");

        UpperState.OnExit(state);
        UpperState = _upperStateTable[state];
        UpperState.LowerBodyStateInfo = LowerState;
        UpperState.OnEnter();
    }


    /****** Protected Members ******/

    protected abstract IPlayerLowerState LowerState { get; set; }
    protected abstract IPlayerUpperState UpperState { get; set; }
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
        Assert.IsTrue(null != _lowerAnimator, $"Lower Animator is not assigned for {CurrentPlayer}.");
        Assert.IsTrue(null != _upperAnimator, $"Upper Animator is not assigned for {CurrentPlayer}.");
        _weapon = GetComponent<PlayerWeaponBase>();
        Assert.IsTrue(null != _weapon, $"Weapon is not assigned for {CurrentPlayer}.");
    }

    private void OnEnable()
    {
        if (false == IsLoaded) return;

        LowerState.OnEnter();
        UpperState.OnEnter();
    }

    private void RegisterStates()
    {
        Assert.IsTrue(null != _playerMotion, $"Player Motion is not assigned for {CurrentPlayer}");
        Assert.IsTrue(null != _playerInfo, $"Player Info is not assigned for {CurrentPlayer}");

        var lowers = GetComponentsInChildren<IPlayerLowerState>();
        Assert.IsTrue(0 < lowers.Length, $"No LowerState components found in children for {CurrentPlayer}");
        foreach (var lower in lowers)
        {
            lower.InitializeState(this, _objectInteractor, _playerMotion, _playerInfo, _lowerAnimator, _weapon);
            LowerStateType state = lower.StateType;
            _lowerStateTable.Add(state, lower);
        }

        var uppers = GetComponentsInChildren<IPlayerUpperState>();
        Assert.IsTrue(0 < uppers.Length, $"No UpperState components found in children for {CurrentPlayer}");
        foreach (var upper in uppers)
        {
            upper.InitializeState(this, _objectInteractor, _playerMotion, _playerInfo, _upperAnimator, _weapon);
            UpperStateType state = upper.StateType;
            _upperStateTable.Add(state, upper);
        }
    }
}
