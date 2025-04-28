using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterEnums;
using UnityEngine.Assertions;

using HeroineLower = PlayerLowerStateBase<CharacterEnums.HeroineLowerState>;
using HeroineUpper = PlayerUpperStateBase<CharacterEnums.HeroineUpperState>;


public class HeroineAvatar : MonoBehaviour, IPlayerAvatar
{
    /****** Public Members ******/

    public bool IsLoaded() => _isLoaded;

    public bool IsAiming { set{} }

    public void InitializeAvatar(PlayerController playerController)
    {
        _playerController = playerController;
        gameObject.SetActive(false);
    }

    public void ControlAvatar(ControlInfo controlInfo)
    {
        ControlLowerBody(controlInfo);
        ControlUpperBody(controlInfo);
    }    

    public void ActivateAvatar(bool value)
    {
        gameObject.SetActive(value);
    }


    /****** Private Members ******/

    private Dictionary<HeroineLowerState, HeroineLower> _lowerStateTable = new Dictionary<HeroineLowerState, HeroineLower>();
    private Dictionary<HeroineUpperState, HeroineUpper> _upperStateTable = new Dictionary<HeroineUpperState, HeroineUpper>();

    private PlayerController        _playerController   = null;
    private HeroineLower            _lowerState         = null;
    private HeroineUpper            _upperState         = null;
    private PlayerAnimatorBase      _animator           = null;

    private bool    _isLoaded   = false;


    private void Awake()
    {
        RegisterStates();
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
        if (_lowerState.ShouldDisableUpperBody())
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
        var lowers = GetComponentsInChildren<HeroineLower>();
        Assert.IsTrue(0 < lowers.Length, "No LowerState components found in children.");
        foreach (var lower in lowers)
        {
            lower.SetOwner(_playerController);
            HeroineLowerState state = lower.GetStateType();
            _lowerStateTable.Add(state, lower);
        }

        var uppers = _playerController.GetComponentsInChildren<HeroineUpper>();
        Assert.IsTrue(0 < uppers.Length, "No HeroineUpperState components found in children.");
        foreach (var upper in uppers)
        {
            upper.SetOwner(_playerController);
            HeroineUpperState state = upper.GetStateType();
            _upperStateTable.Add(state, upper);
        }
    }
}
