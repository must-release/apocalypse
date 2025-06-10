using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using ILowerStateController = IStateController<HeroLowerState>;
using IUpperStateController = IStateController<HeroUpperState>;


public class HeroAvatar : MonoBehaviour, IPlayerAvatar, ILowerStateController, IUpperStateController
{
    /****** Public Members ******/

    public PlayerType PlayerType => PlayerType.Hero;
    public bool IsLoaded
    {
        get
        {
            if (false == _heroWeapon.IsLoaded)
                return false;

            return true;
        }
    }


    public void InitializeAvatar(IMotionController playerMotion, ICharacterInfo playerInfo)
    {
        Assert.IsTrue(null != playerMotion, "Player Physics is null");
        Assert.IsTrue(null != playerInfo, "Player Info is null");

        _playerMotion = playerMotion;
        _playerInfo = playerInfo;

        RegisterStates();
    }

    public void ControlAvatar(ControlInfo controlInfo)
    {
        Assert.IsTrue(null != controlInfo, "Control info is null");

        ControlLowerBody(controlInfo);
        ControlUpperBody(controlInfo);
    }

    public void ActivateAvatar(bool value)
    {
        Assert.IsTrue(_lowerStateTable.ContainsKey(HeroLowerState.Idle), "Idle state not found");
        Assert.IsTrue(_upperStateTable.ContainsKey(HeroUpperState.Idle), "Idle state not found");

        if (false == value)
        {
            _lowerState?.OnExit(HeroLowerState.Idle);
            _upperState?.OnExit(HeroUpperState.Idle);
        }

        _lowerState = _lowerStateTable[HeroLowerState.Idle];
        _upperState = _upperStateTable[HeroUpperState.Idle];

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

        _upperState.OnFixedUpdate();
    }

    public void OnAir()
    {
        _lowerState.OnAir();
        _upperState.OnAir();
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
        ChangeState(HeroLowerState.Dead);
        _upperState.Disable();
    }

    public void ChangeState(HeroLowerState state)
    {
        Assert.IsTrue(_lowerStateTable.ContainsKey(state), "Invalid Lower State");

        Logger.Write(LogCategory.GamePlay, $"Changing Hero Lower State : {_lowerState.StateType} -> {state}", LogLevel.Log, true);

        _lowerState.OnExit(state);
        _lowerState = _lowerStateTable[state];
        _lowerState.OnEnter();

        _upperState.LowerBodyStateInfo = _lowerState;

        if (_lowerState.ShouldDisableUpperBody)
            _upperState.Disable();
        else
            _upperState.Enable();
    }

    public void ChangeState(HeroUpperState state)
    {
        Assert.IsTrue(_upperStateTable.ContainsKey(state), "Invalid Upper State");

        Logger.Write(LogCategory.GamePlay, $"Changing Hero Upper State : {_upperState.StateType} -> {state}", LogLevel.Log, true);

        _upperState.OnExit(state);
        _upperState = _upperStateTable[state];
        _upperState.LowerBodyStateInfo = _lowerState;
        _upperState.OnEnter();
    }


    /****** Private Members ******/

    [SerializeField] private Animator _lowerAnimator;
    [SerializeField] private Animator _upperAnimator;

    private Dictionary<HeroLowerState, HeroLowerStateBase> _lowerStateTable = new();
    private Dictionary<HeroUpperState, HeroUpperStateBase> _upperStateTable = new();

    private IMotionController   _playerMotion;
    private ICharacterInfo      _playerInfo;
    private HeroLowerStateBase  _lowerState;
    private HeroUpperStateBase  _upperState;
    private HeroWeapon          _heroWeapon;

    private void Awake()
    {
        Assert.IsTrue(null != _lowerAnimator, "Lower Animator is not assigned.");
        Assert.IsTrue(null != _upperAnimator, "Upper Animator is not assigned.");
        _heroWeapon = GetComponent<HeroWeapon>();
        Assert.IsTrue(null != _heroWeapon, "Hero Weapon is not assigned.");
    }

    private void OnEnable()
    {
        if (false == IsLoaded) return;

        _lowerState.OnEnter();
        _upperState.OnEnter();
    }

    private void ControlLowerBody(ControlInfo controlInfo)
    {
        // Change player state according to the input control info
        if (controlInfo.move != 0) _lowerState.Move(controlInfo.move);
        else if (controlInfo.stop) _lowerState.Stop();
        if (controlInfo.isJumpStarted) _lowerState.StartJump();
        _lowerState.CheckJumping(controlInfo.isJumping);
        if (controlInfo.tag) _lowerState.Tag();
        _lowerState.Aim(controlInfo.aim);

        // Change player state according to the object control info
        _lowerState.Climb(controlInfo.Climb);
        _lowerState.Push(controlInfo.push);
        _lowerState.UpDown(controlInfo.upDown);
    }

    private void ControlUpperBody(ControlInfo controlInfo)
    {
        if (controlInfo.move != 0) _upperState.Move();
        else if (controlInfo.stop) _upperState.Stop();
        if (controlInfo.isJumpStarted) _upperState.Jump();
        _upperState.Aim(controlInfo.aim);
        if (controlInfo.upDown > 0) _upperState.LookUp(true);
        else _upperState.LookUp(false);
        if (controlInfo.attack) _upperState.Attack();
    }

    private void RegisterStates()
    {
        Assert.IsTrue(null != _playerMotion, "Player Motion is not assigned");
        Assert.IsTrue(null != _playerInfo, "Player Info is not assigned");

        var lowers = GetComponentsInChildren<HeroLowerStateBase>();
        Assert.IsTrue(0 < lowers.Length, "No HeroLowerState components found in children.");
        foreach (var lower in lowers)
        {
            lower.InitializeState(this, _playerMotion, _playerInfo, _lowerAnimator, _heroWeapon);
            HeroLowerState state = lower.StateType;
            _lowerStateTable.Add(state, lower);
        }

        var uppers = GetComponentsInChildren<HeroUpperStateBase>();
        Assert.IsTrue(0 < uppers.Length, "No HeroUpperState components found in children.");
        foreach (var upper in uppers)
        {
            upper.InitializeState(this, _playerMotion, _playerInfo, _upperAnimator, _heroWeapon);
            HeroUpperState state = upper.StateType;
            _upperStateTable.Add(state, upper);
        }
    }
}
