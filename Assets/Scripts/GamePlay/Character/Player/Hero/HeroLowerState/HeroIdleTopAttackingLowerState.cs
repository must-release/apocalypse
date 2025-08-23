using UnityEngine;

public class HeroIdleTopAttackingLowerState : PlayerLowerState
{
    /****** Public Members ******/

    public override LowerStateType CurrentState => HeroLowerStateType.IdleTopAttacking;
    public override bool ShouldDisableUpperBody => true;

    public override void InitializeState(PlayerAvatarType owningAvatar
                                        , IStateController<LowerStateType> stateController
                                        , IObjectInteractor objectInteractor
                                        , IMotionController playerMotion
                                        , ICharacterInfo playerInfo
                                        , Animator stateAnimator
                                        , PlayerWeaponBase playerWeapon
                                        , ControlInputBuffer inputBuffer)
    {
        base.InitializeState(owningAvatar, stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon, inputBuffer);

        Debug.Assert(PlayerAvatarType.Hero == owningAvatar, $"State {CurrentState} can only be used by Hero avatar.");
        Debug.Assert(StateAnimator.HasState(0, _IdleTopAttackingStateHash), $"Animator of {owningAvatar} does not have {CurrentState} upper state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_IdleTopAttackingStateHash, 0, 0);
        StateAnimator.Update(0.0f);

        PlayerWeapon.SetWeaponPivotRotation(90);
        
        _attackCoolTime         = PlayerWeapon.Attack();
        _shouldContinueAttack   = false;
    }

    public override void OnUpdate()
    {
        _attackCoolTime -= Time.deltaTime;

        if (0 < _attackCoolTime) return;

        var nextState = _shouldContinueAttack ? HeroLowerStateType.IdleTopAttacking : HeroLowerStateType.IdleLookingUp;
        StateController.ChangeState(nextState);
    }

    public override void OnExit(LowerStateType nextState)
    {
        PlayerWeapon.SetWeaponPivotRotation(0);
    }

    public override void Attack()
    {

    }

    public override void UpDown(VerticalDirection verticalDirection)
    {
        if (verticalDirection != VerticalDirection.Up)
        {
            StateController.ChangeState(HeroLowerStateType.Idle);
        }
    }

    public override void Move(HorizontalDirection horizontalInput)
    {
        if (HorizontalDirection.None != horizontalInput)
        {
            StateController.ChangeState(HeroLowerStateType.Running);
        }
    }

    public override void OnAir()
    {
        StateController.ChangeState(HeroLowerStateType.Jumping);
    }


    /****** Private Members ******/

    private readonly int _IdleTopAttackingStateHash = AnimatorState.GetHash(PlayerAvatarType.Hero, HeroLowerStateType.IdleTopAttacking);

    private float   _attackCoolTime;
    private bool    _shouldContinueAttack;
}
