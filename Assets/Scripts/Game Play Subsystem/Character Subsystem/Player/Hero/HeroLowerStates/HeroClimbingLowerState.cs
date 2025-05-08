using UnityEngine;

public class HeroClimbingLowerState : PlayerLowerState<HeroLowerState>
{
    /****** Public Members ******/

    public override HeroLowerState  StateType               => HeroLowerState.Climbing;
    public override bool            ShouldDisableUpperBody  => true;

    public override void InitializeState(IStateController<HeroLowerState> stateController, 
                                         IMotionController playerPhysics, 
                                         ICharacterInfo playerInfo, 
                                         Animator StateAnimator,
                                         PlayerWeaponBase playerWeapon
    )
    {
        base.InitializeState(stateController, playerPhysics, playerInfo, StateAnimator, playerWeapon);

        _climbingSpeed      = playerInfo.MovingSpeed;
        _climbUpHeight      = 0.1f;
        _climbDownHeight    = playerInfo.CharacterHeight / 2 + 0.2f;
    }

    public override void OnEnter()
    {
        PlayerMotion.SetGravityScale(0);

        MoveNearToClimbingObject();
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {
        PlayerMotion.SetGravityScale(PlayerInfo.Gravity);
    }

    public override void UpDown(int upDown)
    {
        PlayerMotion.SetVelocity(Vector2.up * upDown * _climbingSpeed);
    }
    
    public override void Climb(bool climb)
    {
        if (climb) return;

        if (0 < PlayerInfo.CurrentVelocity.y)
        {
            // Move player on the upside of the ladder
            PlayerMotion.TeleportTo(PlayerInfo.CurrentPosition + Vector2.up * PlayerInfo.CharacterHeight / 2);

            StateController.ChangeState(HeroLowerState.Idle);
        }
        else
        {
            // Climbed down the climing object
            var nextState = PlayerInfo.StandingGround == null ? HeroLowerState.Jumping : HeroLowerState.Idle;
            StateController.ChangeState(nextState);
        }
    }

    public override void Jump()
    {
        PlayerMotion.SetVelocity(new Vector2(PlayerInfo.CurrentVelocity.x, PlayerInfo.JumpingSpeed / 3));

        StateController.ChangeState(HeroLowerState.Jumping);
    }


    /****** Private Members ******/

    private float _climbingSpeed    = 0.0f;
    private float _climbUpHeight    = 0.0f;
    private float _climbDownHeight  = 0.0f;

    private void MoveNearToClimbingObject()
    {
        Transform climbingObject = PlayerInfo.CurrentControlInfo.climbingObject.transform;

        float offset = 0 < PlayerInfo.CurrentControlInfo.upDown ? _climbUpHeight :  -_climbDownHeight;

        PlayerMotion.TeleportTo(new Vector2(climbingObject.position.x, PlayerInfo.CurrentPosition.y + offset));
    }
}
