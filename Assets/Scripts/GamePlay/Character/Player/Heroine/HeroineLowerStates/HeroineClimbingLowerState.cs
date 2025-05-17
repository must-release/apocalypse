using UnityEngine;

public class HeroineClimbingLowerState : HeroineLowerStateBase
{
    /****** Public Members ******/

    public override HeroineLowerState   StateType               => HeroineLowerState.Climbing;
    public override bool                ShouldDisableUpperBody  => true;

    public override void InitializeState(IStateController<HeroineLowerState> stateController, 
                                         IMotionController playerPhysics, 
                                         ICharacterInfo playerInfo, 
                                         Animator stateAnimator,
                                         PlayerWeaponBase playerWeapon
    )
    {
        base.InitializeState(stateController, playerPhysics, playerInfo, stateAnimator, playerWeapon);

        _climbingSpeed      = playerInfo.MovingSpeed * 2.0f / 3.0f;
        _climbUpHeight      = 0.1f;
        _climbDownHeight    = playerInfo.CharacterHeight / 2 + 0.2f;
    }

    public override void OnEnter()
    {
        PlayerMotion.SetGravityScale(0);
        StateAnimator.Play(AnimatorState.HeroineLower.ClimbingUp);

        _climbingObject = PlayerInfo.CurrentControlInfo.climbingObject.transform;

        MoveNearToClimbingObject();
    }

    public override void OnUpdate()
    {
        if (false == Mathf.Approximately(PlayerInfo.CurrentPosition.x, _climbingObject.position.x))
            PlayerMotion.TeleportTo(new Vector2(_climbingObject.position.x , PlayerInfo.CurrentPosition.y));

        float verticalVelocity = PlayerInfo.CurrentVelocity.y;

        if (Mathf.Approximately(verticalVelocity, 0f))
        {
            StateAnimator.speed = 0.0f;
        }
        else
        {
            var nextClip = verticalVelocity > 0 ? AnimatorState.HeroineLower.ClimbingUp : AnimatorState.HeroineLower.ClimbingDown;

            if (!StateAnimator.GetCurrentAnimatorStateInfo(0).IsName(nextClip.ToString()))
                StateAnimator.Play(nextClip);

            StateAnimator.speed = 1.0f;
        }
    }

    public override void OnExit()
    {
        PlayerMotion.SetGravityScale(PlayerInfo.Gravity);
        StateAnimator.speed = 1.0f;
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

            StateController.ChangeState(HeroineLowerState.Idle);
        }
        else
        {
            // Climbed down the climbing object
            var nextState = PlayerInfo.StandingGround == null ? HeroineLowerState.Jumping : HeroineLowerState.Idle;
            StateController.ChangeState(nextState);
        }
    }

    public override void StartJump()
    {
        PlayerMotion.SetVelocity(new Vector2(PlayerInfo.CurrentVelocity.x, PlayerInfo.JumpingSpeed / 3));

        StateController.ChangeState(HeroineLowerState.Jumping);
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroineLowerState.Damaged);
    }


    /****** Protected Members ******/

    protected override string AnimationClipPath => AnimationClipAsset.HeroineLower.ClimbingUp;


    /****** Private Members ******/

    private float _climbingSpeed    = 0.0f;
    private float _climbUpHeight    = 0.0f;
    private float _climbDownHeight  = 0.0f;
    private Transform _climbingObject = null;

    private void MoveNearToClimbingObject()
    {
        float offset = 0 < PlayerInfo.CurrentControlInfo.upDown ? _climbUpHeight :  -_climbDownHeight;

        PlayerMotion.TeleportTo(new Vector2(_climbingObject.position.x, PlayerInfo.CurrentPosition.y + offset));
    }
}
