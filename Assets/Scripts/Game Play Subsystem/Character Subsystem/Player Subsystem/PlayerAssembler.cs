using UnityEngine;
using CharacterEums;
using UnityEngine.Assertions;

public class PlayerAssembler
{
    /****** Public Members ******/

    public PlayerAssembler(PlayerController playerController, Transform heroTransform, Transform heroineTransfomr)
    {
        Assert.IsTrue(null != playerController, "PlayerController is null");
        Assert.IsTrue(null != heroTransform, "HeroTransform is null");
        Assert.IsTrue(null != heroineTransfomr, "HeroineTransform is null");

        _playerController   = playerController;
        _heroTransform      = heroTransform;
        _heroineTransform   = heroineTransfomr;
    }

    public void Assemble()
    {
        RegisterAvatar(PlayerType.Hero, _heroTransform);
        RegisterAvatar(PlayerType.Heroine, _heroineTransform);

        //RegisterAnimator(PlayerType.Hero, _heroTransform);
        RegisterAnimator(PlayerType.Heroine, _heroineTransform);

        RegisterStates();
    }



    /****** Private Members ******/

    private PlayerController _playerController  = null;
    private Transform _heroTransform            = null;
    private Transform _heroineTransform         = null;

    private void RegisterAvatar(PlayerType type, Transform root)
    {
        IPlayerAvatar avatar = root.GetComponent<IPlayerAvatar>();
        Assert.IsTrue(null != avatar, $"{type} avatar (IPlayerAvatar) not found in {root.name}");
        _playerController.RegisterAvatar(type, avatar);
        avatar.InitializeAvatar(_playerController);
    }

    private void RegisterAnimator(PlayerType type, Transform root)
    {
        PlayerAnimatorBase animator = root.GetComponent<PlayerAnimatorBase>();
        Assert.IsTrue(null != animator, $"{type} animator not found in {root.name}");
        _playerController.RegisterAnimator(type, animator);
    }

    private void RegisterStates()
    {
        var lowerStates = _playerController.GetComponentsInChildren<PlayerLowerStateBase>();
        Assert.IsTrue( 0 < lowerStates.Length, "No PlayerLowerStateBase components found in children of PlayerController.");
        foreach (var lowerState in lowerStates)
        {
            lowerState.SetOwner(_playerController);
            CommonPlayerLowerState state = lowerState.GetStateType();
            _playerController.RegisterLowerState(state, lowerState);
        }

        var upperStates = _playerController.GetComponentsInChildren<PlayerUpperStateBase>();
        Assert.IsTrue(0 < upperStates.Length, "No PlayerUpperStateBase components found in children of PlayerController.");
        foreach (var upperState in upperStates)
        {
            upperState.SetOwner(_playerController);
            CommonPlayerUpperState state = upperState.GetStateType();
            _playerController.RegisterUpperState(state, upperState);
        }
    }
}
