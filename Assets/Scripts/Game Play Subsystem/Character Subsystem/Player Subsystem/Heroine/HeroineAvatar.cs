using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterEnums;
using UnityEngine.Assertions;

using HeroineLower              = PlayerLowerStateBase<CharacterEnums.HeroineLowerState>;
using HeroineUpper              = PlayerUpperStateBase<CharacterEnums.HeroineUpperState>;
using ILowerStateController     = IStateController<CharacterEnums.HeroineLowerState>;
using IUpperStateController     = IStateController<CharacterEnums.HeroineUpperState>;


public class HeroineAvatar : MonoBehaviour, IPlayerAvatar, ILowerStateController, IUpperStateController, IAsyncLoadObject
{
    /****** Public Members ******/

    public PlayerType   PlayerType  => PlayerType.Heroine;
    public bool         IsLoaded    => _isLoaded;


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

        if (value)
        {
            _lowerState = _lowerStateTable[HeroineLowerState.Idle];
            _upperState = _upperStateTable[HeroineUpperState.Idle];
        }

        gameObject.SetActive(value);
    }

    public void OnUpdate()
    {
        Assert.IsTrue(_isLoaded, "Avatar is not loaded yet");

        _lowerState.OnUpdate();
        _upperState.OnUpdate();
    }

    public void OnAir()
    {
        _lowerState.OnAir();
        _upperState.OnAir();
    }

    public void OnGround()
    {
        _lowerState.OnGround();
        _upperState.OnGround();
    }

    public void OnDamaged(DamageInfo damageInfo)
    {
        _lowerState.Damaged();
        _upperState.Disable();
    }

    public void ChangeState(HeroineLowerState state)
    {
        Assert.IsTrue(_lowerStateTable.ContainsKey(state), "Invalid Lower State");

        Debug.Log("Lower : " + _lowerState.StateType.ToString() + " -> " + state.ToString());

        _lowerState.OnExit();
        _lowerState = _lowerStateTable[state];
        _lowerState.OnEnter();
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

    private Dictionary<HeroineLowerState, HeroineLower> _lowerStateTable = new Dictionary<HeroineLowerState, HeroineLower>();
    private Dictionary<HeroineUpperState, HeroineUpper> _upperStateTable = new Dictionary<HeroineUpperState, HeroineUpper>();

    private IMotionController       _playerMotion       = null;
    private ICharacterInfo          _playerInfo         = null;
    private HeroineLower            _lowerState         = null;
    private HeroineUpper            _upperState         = null;
    private PlayerAnimatorBase      _animator           = null;

    private bool    _isLoaded   = false;


    private void Awake()
    {
        _animator = GetComponent<PlayerAnimatorBase>();
    }

    private IEnumerator Start() 
    {
        yield return LoadWeaponsAndDots(); 
    }

    private IEnumerator LoadWeaponsAndDots()
    {
        _isLoaded = true;

        yield return null;
    }

    private void ControlLowerBody(ControlInfo controlInfo)
    {
        // Change player state according to the input control info
        if (controlInfo.move != 0) _lowerState.Move(controlInfo.move);
        else if (controlInfo.stop) _lowerState.Stop();
        if (controlInfo.jump) _lowerState.Jump();
        if (controlInfo.tag) _lowerState.Tag();
        _lowerState.Aim(controlInfo.aim != Vector3.zero);
        _lowerState.UpDown(controlInfo.upDown);

        // Change player state according to the object control info
        _lowerState.Climb(controlInfo.climb);
        _lowerState.Push(controlInfo.push);
    }

    private void ControlUpperBody(ControlInfo controlInfo)
    {   
        if (_lowerState.ShouldDisableUpperBody)
        {
            _upperState.Disable();
            return;
        }
        
        _upperState.Enable();
        if (controlInfo.move != 0) _upperState.Move();
        else if (controlInfo.stop) _upperState.Stop();
        if (controlInfo.jump) _upperState.Jump();
        _upperState.Aim(controlInfo.aim);
        if (controlInfo.upDown > 0) _upperState.LookUp(true);
        else _upperState.LookUp(false);
        if (controlInfo.attack) _upperState.Attack();
    }

    private void RegisterStates()
    {
        Assert.IsTrue(null != _playerMotion, "Player Motion is not assigned");
        Assert.IsTrue(null != _playerInfo, "Player Info is not assigned");
        Assert.IsTrue(null != _animator, "Animator is not assigned");

        var lowers = GetComponentsInChildren<HeroineLower>();
        Assert.IsTrue(0 < lowers.Length, "No LowerState components found in children.");
        foreach (var lower in lowers)
        {
            lower.InitializeState(this ,_playerMotion, _playerInfo, _animator);
            HeroineLowerState state = lower.StateType;
            _lowerStateTable.Add(state, lower);
        }

        var uppers = GetComponentsInChildren<HeroineUpper>();
        Assert.IsTrue(0 < uppers.Length, "No HeroineUpperState components found in children.");
        foreach (var upper in uppers)
        {
            upper.InitializeState(this, _playerMotion, _playerInfo, _animator);
            HeroineUpperState state = upper.StateType;
            _upperStateTable.Add(state, upper);
        }
    }
}
