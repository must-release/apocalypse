using NUnit.Framework;
using UnityEngine;

public class HeroClimbingLowerState : HeroLowerStateBase
{
    /****** Public Members ******/

    public override HeroLowerState StateType    => HeroLowerState.Climbing;
    public override bool ShouldDisableUpperBody => true;

    public override void InitializeState(IStateController<HeroLowerState> stateController, IMotionController playerPhysics, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, playerPhysics, playerInfo, stateAnimator, playerWeapon);

        Assert.IsTrue(StateAnimator.HasState(0, _ClimbingDownStateHash), "Hero animator does not have climbing down lower state.");
        Assert.IsTrue(StateAnimator.HasState(0, _ClimbingUpStateHash), "Hero animator does not have climbing up lower state.");

        _climbingSpeed      = playerInfo.MovingSpeed * 2.0f / 3.0f;
        _climbUpHeight      = 0.1f;
        _climbDownHeight    = playerInfo.CharacterHeight / 2 + 0.2f;
    }

    public override void OnEnter()
    {
        PlayerMotion.SetGravityScale(0);
        StateAnimator.Play(_ClimbingUpStateHash);

        _climbingObject = PlayerInfo.CurrentControlInfo.climbingObject.transform;

        MoveNearToClimbingObject();
    }

    public override void OnUpdate()
    {
        if (false == Mathf.Approximately(PlayerInfo.CurrentPosition.x, _climbingObject.position.x))
        {
            PlayerMotion.TeleportTo(new Vector2(_climbingObject.position.x, PlayerInfo.CurrentPosition.y));
        }

        float verticalVelocity = PlayerInfo.CurrentVelocity.y;
        if (Mathf.Approximately(verticalVelocity, 0f))
        {
            StateAnimator.speed = 0.0f;
        }
        else
        {
            var nextClipHash = verticalVelocity > 0 ? _ClimbingUpStateHash : _ClimbingDownStateHash;

            if (StateAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash != nextClipHash)
            {
                StateAnimator.Play(nextClipHash);
            }

            StateAnimator.speed = 1.0f;
        }
    }

    public override void OnExit(HeroLowerState _)
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
            PlayerMotion.TeleportTo(PlayerInfo.CurrentPosition + Vector3.up * PlayerInfo.CharacterHeight / 2);

            StateController.ChangeState(HeroLowerState.Idle);
        }
        else
        {
            // Climbed down the climbing object
            var nextState = PlayerInfo.StandingGround == null ? HeroLowerState.Jumping : HeroLowerState.Idle;
            StateController.ChangeState(nextState);
        }
    }

    public override void StartJump()
    {
        PlayerMotion.SetVelocity(new Vector2(PlayerInfo.CurrentVelocity.x, PlayerInfo.JumpingSpeed / 3));

        StateController.ChangeState(HeroLowerState.Jumping);
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroLowerState.Damaged);
    }


    /****** Private Members ******/

    private readonly int _ClimbingUpStateHash   = AnimatorState.Hero.GetHash(HeroLowerState.Climbing, "Down");
    private readonly int _ClimbingDownStateHash = AnimatorState.Hero.GetHash(HeroLowerState.Climbing, "Up");

    private float _climbingSpeed;
    private float _climbUpHeight;
    private float _climbDownHeight;

    private Transform _climbingObject;

    private void MoveNearToClimbingObject()
    {
        float offset = 0 < PlayerInfo.CurrentControlInfo.upDown ? _climbUpHeight : -_climbDownHeight;

        PlayerMotion.TeleportTo(new Vector2(_climbingObject.position.x, PlayerInfo.CurrentPosition.y + offset));
    }
}
