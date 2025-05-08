using UnityEngine;

public class HeroineAimingUpperState : PlayerUpperStateBase<HeroineUpperState>
{
    /****** Public Members ******/

    public override HeroineUpperState StateType => HeroineUpperState.Aiming;

    public override void OnEnter()
    {
        // Enable fixed update
        _fixedUpdateFlag = true;
    }

    public override void OnUpdate()
    {
        // Set player's looking direction
        SetDirection();

        // Rotate upper body toward the aiming position
        //OwnerController.CurrentAvatar.RotateUpperBody(_aimingPosition);
    }

    private void FixedUpdate() 
    {   
        if(false == _fixedUpdateFlag) return;

        //OwnerController.CurrentAvatar.Aim(true);
    }

    public override void OnExit(HeroineUpperState nextState)
    {
        // Turn off player's aiming state
        //OwnerController.CurrentAvatar.Aim(false);

        // Recover player's upper body rotation when not attacking
        // if(nextState != HeroineUpperState.AimAttacking)
        //     OwnerController.CurrentAvatar.RotateUpperBody(0);

        // Disable fixed update
        _fixedUpdateFlag = false;
    }

    public override void Attack() 
    {
        StateController.ChangeState(HeroineUpperState.Attacking); 
    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim)
            StateController.ChangeState(HeroineUpperState.Idle); 
        else
            _aimingPosition = aim;
    }
    public override void Disable()
    {
        StateController.ChangeState(HeroineUpperState.Disabled);
    }


    /****** Protected Members ******/

    protected override string AnimationClipPath => AnimationClipAsset.HeroineUpper.Aiming;


    /****** Private Members ******/

    private Vector3 _aimingPosition     = Vector3.zero;
    private bool    _fixedUpdateFlag    = false;

    private void SetDirection()
    {
        var direction = PlayerInfo.CurrentPosition.x < _aimingPosition.x ? FacingDirection.Right : FacingDirection.Left;
        if (direction != PlayerInfo.FacingDirection)
        {
            PlayerMotion.SetFacingDirection(direction);
        }
    }
}
