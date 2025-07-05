using UnityEngine;
using UnityEngine.Assertions;

public class HeroineClimbingLowerState : HeroineLowerState
{
    /****** Public Members ******/

    public override HeroineLowerStateType   StateType               => HeroineLowerStateType.Climbing;
    public override bool                ShouldDisableUpperBody  => true;

    public override void InitializeState(IStateController<HeroineLowerStateType> stateController,
                                        IObjectInteractor objectInteractor,
                                        IMotionController playerPhysics,
                                        ICharacterInfo playerInfo,
                                        Animator stateAnimator,
                                        PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, objectInteractor, playerPhysics, playerInfo, stateAnimator, playerWeapon);

        Assert.IsTrue(StateAnimator.HasState(0, _ClimbingDownStateHash), "Animator does not have climbing down state.");
        Assert.IsTrue(StateAnimator.HasState(0, _ClimbingUpStateHash), "Animator does not have climbing up state.");

        _climbingSpeed      = playerInfo.MovingSpeed * 2.0f / 3.0f;
        _climbUpHeight      = 0.1f;
        _climbDownHeight    = playerInfo.CharacterHeight / 2 + 0.2f;
    }

    public override void OnEnter()
    {
        _recentMoveDirection = VerticalDirection.None;

        PlayerMotion.SetGravityScale(0);
        StateAnimator.Play(_ClimbingUpStateHash);

        _climbingObject = ObjectInteractor.CurrentClimbableObject;
        _climbingObject.OnClimbStart(ObjectInteractor);

        MoveNearToClimbingObject();
    }

    public override void OnUpdate()
    {
        ControlClimbingAnimation();
        CheckIfCanClimbFurther();
    }

    public override void OnExit(HeroineLowerStateType _)
    {
        PlayerMotion.SetGravityScale(PlayerInfo.Gravity);
        _climbingObject.OnClimbEnd(ObjectInteractor); 
        StateAnimator.speed = 1.0f;
    }

    public override void UpDown(VerticalDirection verticalInput)
    {
        PlayerMotion.SetVelocity(Vector2.up * (int)verticalInput * _climbingSpeed);
        _recentMoveDirection = verticalInput;
    }

    public override void StartJump()
    {
        PlayerMotion.SetVelocity(new Vector2(PlayerInfo.CurrentVelocity.x, PlayerInfo.JumpingSpeed / 3));

        StateController.ChangeState(HeroineLowerStateType.Jumping);
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroineLowerStateType.Damaged);
    }

    public override void OnGround()
    {
        StateController.ChangeState(HeroineLowerStateType.Idle);
    }


    /****** Private Members ******/

    private readonly int _ClimbingUpStateHash   = AnimatorState.Heroine.GetHash(HeroineLowerStateType.Climbing, "Down");
    private readonly int _ClimbingDownStateHash = AnimatorState.Heroine.GetHash(HeroineLowerStateType.Climbing, "Up");

    private float   _climbingSpeed;
    private float   _climbUpHeight;
    private float   _climbDownHeight;

    private VerticalDirection _recentMoveDirection;
    private IClimbable _climbingObject;

    private void MoveNearToClimbingObject()
    {
        float offset = (PlayerInfo.CurrentPosition.y < _climbingObject.GetClimbReferencePoint().y) ? _climbUpHeight :  -_climbDownHeight;

        PlayerMotion.TeleportTo(new Vector2(_climbingObject.GetClimbReferencePoint().x, PlayerInfo.CurrentPosition.y + offset));
    }

    private void ControlClimbingAnimation()
    {
        if (VerticalDirection.None == _recentMoveDirection)
        {
            StateAnimator.speed = 0.0f;
        }
        else
        {
            var nextClipHash = (VerticalDirection.Up == _recentMoveDirection) ? _ClimbingUpStateHash : _ClimbingDownStateHash;

            if (StateAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != nextClipHash)
            {
                StateAnimator.Play(nextClipHash);
            }

            StateAnimator.speed = 1.0f;
        }
    }

    private void CheckIfCanClimbFurther()
    {
        if (VerticalDirection.None == _recentMoveDirection || _climbingObject.CanClimbFurther(PlayerInfo.CurrentPosition, _recentMoveDirection))
            return;

        if (VerticalDirection.Up == _recentMoveDirection)
        {
            // Movement player on the upside of the ladder
            PlayerMotion.TeleportTo(PlayerInfo.CurrentPosition + Vector3.up * (PlayerInfo.CharacterHeight / 2 + 0.8f));
            StateController.ChangeState(HeroineLowerStateType.Idle);
        }
        else
        {
            // Climbed down the climbing object
            var nextState = PlayerInfo.StandingGround == null ? HeroineLowerStateType.Jumping : HeroineLowerStateType.Idle;
            StateController.ChangeState(nextState);
        }
    }
}
