using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using ILowerStateController     = IStateController<HeroineLowerState>;
using IUpperStateController     = IStateController<HeroineUpperState>;


public class HeroineAvatar : MonoBehaviour, IPlayerAvatar, ILowerStateController, IUpperStateController
{
    /****** Public Members ******/

    public PlayerType   PlayerType  => PlayerType.Heroine;
    public bool         IsLoaded
    {
        get
        {
            if (false == _heroineWeapon.IsLoaded)
                return false;

            return true;
        }
    }


    public void InitializeAvatar(IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo)
    {
        Assert.IsTrue(null != objectInteractor, "Object interactor is null");
        Assert.IsTrue(null != playerMotion, "Player Physics is null");
        Assert.IsTrue(null != playerInfo, "Player Info is null");

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
        Assert.IsTrue(_lowerStateTable.ContainsKey(HeroineLowerState.Idle), "Idle state not found");
        Assert.IsTrue(_upperStateTable.ContainsKey(HeroineUpperState.Idle), "Idle state not found");

        if (false == value)
        {
            _lowerState?.OnExit(HeroineLowerState.Idle);
            _upperState?.OnExit(HeroineUpperState.Idle);
        }

        _lowerState = _lowerStateTable[HeroineLowerState.Idle];
        _upperState = _upperStateTable[HeroineUpperState.Idle];
        _upperState.LowerBodyStateInfo = _lowerState;

        gameObject.SetActive(value);
    }

    public void OnUpdate()
    {
        Assert.IsTrue(IsLoaded, "Avatar is not loaded yet");

        _lowerState.OnUpdate();
        _upperState.OnUpdate();
    }

    public void OnFixedUpdate()
    {
        Assert.IsTrue(IsLoaded, "Avatar is not loaded yet");

        _lowerState.OnFixedUpdate();
    }

    public void OnAir()
    {
        _lowerState.OnAir();
    }

    public void OnGround()
    {
        _lowerState.OnGround();
    }

    public void OnDamaged(DamageInfo damageInfo)
    {
        _lowerState.Damaged();
        _upperState.Disable();
    }

    public void OnDead()
    {
        ChangeState(HeroineLowerState.Dead);
        _upperState.Disable();
    }

    public void ChangeState(HeroineLowerState state)
    {
        Assert.IsTrue(_lowerStateTable.ContainsKey(state), "Invalid Lower State");

        Logger.Write(LogCategory.GamePlay, $"{_lowerState} -> {state}");

        _lowerState.OnExit(state);
        _lowerState = _lowerStateTable[state];
        _lowerState.OnEnter();

        _upperState.LowerBodyStateInfo = _lowerState;

        if (_lowerState.ShouldDisableUpperBody)
            _upperState.Disable();
        else
            _upperState.Enable();
    }

    public void ChangeState(HeroineUpperState state)
    {
        Assert.IsTrue(_upperStateTable.ContainsKey(state), "Invalid Upper State");

        _upperState.OnExit(state);
        _upperState = _upperStateTable[state];
        _upperState.LowerBodyStateInfo = _lowerState;
        _upperState.OnEnter();
    }


    /****** Private Members ******/

    [SerializeField] private Animator _lowerAnimator;
    [SerializeField] private Animator _upperAnimator;

    private Dictionary<HeroineLowerState, HeroineLowerStateBase> _lowerStateTable = new Dictionary<HeroineLowerState, HeroineLowerStateBase>();
    private Dictionary<HeroineUpperState, HeroineUpperStateBase> _upperStateTable = new Dictionary<HeroineUpperState, HeroineUpperStateBase>();

    private IObjectInteractor       _objectInteractor;
    private IMotionController       _playerMotion;
    private ICharacterInfo          _playerInfo;
    private HeroineLowerStateBase   _lowerState;
    private HeroineUpperStateBase   _upperState;
    private HeroineWeapon           _heroineWeapon;

    private void Awake()
    {
        Assert.IsTrue(null != _lowerAnimator, "Lower Animator is not assigned.");
        Assert.IsTrue(null != _upperAnimator, "Upper Animator is not assigned.");
        _heroineWeapon = GetComponent<HeroineWeapon>();
        Assert.IsTrue(null != _heroineWeapon, "Heroine Weapon is not assigned.");
    }

    private void OnEnable()
    {
        if (false == IsLoaded) return;

        _lowerState.OnEnter();
        _upperState.OnEnter();
    }

    private void ControlLowerBody(IReadOnlyControlInfo controlInfo)
    {
        _lowerState.Move(controlInfo.HorizontalInput);
        if (controlInfo.IsJumpStarted) _lowerState.StartJump();
        _lowerState.CheckJumping(controlInfo.IsJumping);
        if (controlInfo.IsTagging) _lowerState.Tag();
        _lowerState.Aim(controlInfo.AimingPosition);
        if (controlInfo.IsAttacking) _lowerState.Attack();
        _lowerState.UpDown(controlInfo.VerticalInput);
    }

    private void ControlUpperBody(IReadOnlyControlInfo controlInfo)
    {
        _upperState.Move(controlInfo.HorizontalInput);
        if (controlInfo.IsJumpStarted) _upperState.Jump();
        _upperState.Aim(controlInfo.AimingPosition);
        if (VerticalDirection.Up == controlInfo.VerticalInput) _upperState.LookUp(true);
        else _upperState.LookUp(false);
    }

    private void RegisterStates()
    {
        Assert.IsTrue(null != _playerMotion, "Player Motion is not assigned");
        Assert.IsTrue(null != _playerInfo, "Player Info is not assigned");

        var lowers = GetComponentsInChildren<HeroineLowerStateBase>();
        Assert.IsTrue(0 < lowers.Length, "No HeroineLowerState components found in children.");
        foreach (var lower in lowers)
        {
            lower.InitializeState(this, _objectInteractor, _playerMotion, _playerInfo, _lowerAnimator, _heroineWeapon);
            HeroineLowerState state = lower.StateType;
            _lowerStateTable.Add(state, lower);
        }

        var uppers = GetComponentsInChildren<HeroineUpperStateBase>();
        Assert.IsTrue(0 < uppers.Length, "No HeroineUpperState components found in children.");
        foreach (var upper in uppers)
        {
            upper.InitializeState(this, _objectInteractor, _playerMotion, _playerInfo, _upperAnimator, _heroineWeapon);
            HeroineUpperState state = upper.StateType;
            _upperStateTable.Add(state, upper);
        }
    }
}
