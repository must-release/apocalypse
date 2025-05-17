using System.Collections;
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

            foreach (var lower in _lowerStateTable.Values)
            {
                if (false == lower.IsLoaded)
                    return false;
            }

            foreach (var upper in _upperStateTable.Values)
            {
                if (false == upper.IsLoaded)
                    return false;
            }

            return true;
        }
    }


    public void InitializeAvatar(IMotionController playerMotion, ICharacterInfo playerInfo)
    {
        Assert.IsTrue(null != playerMotion, "Player Physics is null");
        Assert.IsTrue(null != playerInfo, "Player Info is null");

        _playerMotion   = playerMotion;
        _playerInfo     = playerInfo;

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
        Assert.IsTrue(_lowerStateTable.ContainsKey(HeroineLowerState.Idle), "Idle state not found");
        Assert.IsTrue(_upperStateTable.ContainsKey(HeroineUpperState.Idle), "Idle state not found");

        gameObject.SetActive(value);

        if (value)
        {
            _lowerState = _lowerStateTable[HeroineLowerState.Idle];
            _upperState = _upperStateTable[HeroineUpperState.Idle];
        }
    }

    public void OnUpdate()
    {
        Assert.IsTrue(IsLoaded, "Avatar is not loaded yet");

        _lowerState.OnUpdate();
        _upperState.OnUpdate();
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

        Debug.Log("Lower : " + _lowerState.StateType.ToString() + " -> " + state.ToString());

        _lowerState.OnExit();
        _lowerState = _lowerStateTable[state];
        _lowerState.OnEnter();

        if (_lowerState.ShouldDisableUpperBody)
            _upperState.Disable();
        else
            _upperState.Enable();
    }

    public void ChangeState(HeroineUpperState state)
    {
        Assert.IsTrue(_upperStateTable.ContainsKey(state), "Invalid Upper State");

        Debug.Log("Upper : " + _upperState.StateType.ToString() + " -> " + state.ToString());

        _upperState.OnExit(state);
        _upperState = _upperStateTable[state];
        _upperState.OnEnter();
    }


    /****** Private Members ******/

    [SerializeField] private Animator _lowerAnimator = null;
    [SerializeField] private Animator _upperAnimator = null;

    private Dictionary<HeroineLowerState, HeroineLowerStateBase> _lowerStateTable = new Dictionary<HeroineLowerState, HeroineLowerStateBase>();
    private Dictionary<HeroineUpperState, HeroineUpperStateBase> _upperStateTable = new Dictionary<HeroineUpperState, HeroineUpperStateBase>();

    private IMotionController       _playerMotion       = null;
    private ICharacterInfo          _playerInfo         = null;
    private HeroineLowerStateBase   _lowerState         = null;
    private HeroineUpperStateBase   _upperState         = null;
    private HeroineWeapon           _heroineWeapon      = null;

    private void Awake()
    {
        Assert.IsTrue(null != _lowerAnimator, "Lower Animator is not assigned.");
        Assert.IsTrue(null != _upperAnimator, "Upper Animator is not assigned.");
        _heroineWeapon = GetComponentInChildren<HeroineWeapon>();
        Assert.IsTrue(null != _heroineWeapon, "Heroine Weapon is not assigned.");
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
        _lowerState.Aim(controlInfo.aim != Vector3.zero);
        if (controlInfo.attack) _lowerState.Attack();

        // Change player state according to the object control info
        _lowerState.Climb(controlInfo.climb);
        _lowerState.Push(controlInfo.push);
        _lowerState.UpDown(controlInfo.upDown);
    }

    private void ControlUpperBody(ControlInfo controlInfo)
    {  
        if (controlInfo.move != 0) _upperState.Move();
        else if (controlInfo.stop) _upperState.Stop();
        if (controlInfo.isJumping) _upperState.Jump();
        _upperState.Aim(controlInfo.aim);
        if (controlInfo.upDown > 0) _upperState.LookUp(true);
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
            lower.InitializeState(this ,_playerMotion, _playerInfo, _lowerAnimator, _heroineWeapon);
            HeroineLowerState state = lower.StateType;
            _lowerStateTable.Add(state, lower);
        }

        var uppers = GetComponentsInChildren<HeroineUpperStateBase>();
        Assert.IsTrue(0 < uppers.Length, "No HeroineUpperState components found in children.");
        foreach (var upper in uppers)
        {
            upper.InitializeState(this, _playerMotion, _playerInfo, _upperAnimator, _heroineWeapon);
            HeroineUpperState state = upper.StateType;
            _upperStateTable.Add(state, upper);
        }
    }
}
