    using UnityEditor;
using UnityEngine;

public class HeroineJumpingLowerState : HeroineLowerStateBase
{
    /****** Public Members ******/

    public override HeroineLowerState   StateType               => HeroineLowerState.Jumping;
    public override bool                ShouldDisableUpperBody  => true;

    public override void OnEnter()
    {
        StateAnimator.Play(AnimatorState.HeroineLower.Jumping);

        _isJumping = true;
    }

    public override void OnUpdate()
    {
        if (false == _isJumping && 0 < PlayerInfo.CurrentVelocity.y)
        {
            PlayerMotion.SetVelocity(new Vector2(PlayerInfo.CurrentVelocity.x, PlayerInfo.CurrentVelocity.y - 10 * PlayerInfo.JumpingSpeed * Time.deltaTime));
        }
    }

    public override void OnExit()
    {

    }

    public override void Move(int move)
    {
        if (0 == move) return;

        // move player
        PlayerMotion.SetVelocity(new Vector2(move * PlayerInfo.MovingSpeed, PlayerInfo.CurrentVelocity.y));

        // set direction
        FacingDirection direction = move < 0 ? FacingDirection.Left : FacingDirection.Right;
        if (direction != PlayerInfo.FacingDirection)
        {
            PlayerMotion.SetFacingDirection(direction);
        }
    }

    public override void Stop()
    {
        PlayerMotion.SetVelocity(new Vector2(0, PlayerInfo.CurrentVelocity.y));
    }

    public override void Attack()
    {
        StateController.ChangeState(HeroineLowerState.Attacking);
    }

    public override void OnGround()
    {
        StateController.ChangeState(HeroineLowerState.Idle);
    }

    public override void Climb(bool climb) 
    {
        if (false == climb) return;

        StateController.ChangeState(HeroineLowerState.Climbing);
    }

    public override void CheckJumping(bool isJumping)
    {
        if (false == _isJumping) 
            return;


        if (false == isJumping)
            _isJumping = false;
    }

    public override void Damaged()
    {
        StateController.ChangeState(HeroineLowerState.Damaged);
    }


    /****** Protected Members ******/

    protected override string AnimationClipPath => AnimationClipAsset.HeroineLower.Jumping;

    /****** Private Members ******/

    private bool _isJumping = false;
}
