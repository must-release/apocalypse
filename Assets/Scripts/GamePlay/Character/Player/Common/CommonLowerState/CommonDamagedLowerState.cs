using UnityEngine;
using UnityEngine.Assertions;

public class CommonDamagedLowerState : PlayerLowerState
{
    /****** Public Members ******/

    public override LowerStateType CurrentState => LowerStateType.Damaged;
    public override bool ShouldDisableUpperBody => true;

    public override void InitializeState(PlayerAvatarType owningAvatar
                                         , IStateController<LowerStateType> stateController
                                         , IObjectInteractor objectInteractor
                                         , IMotionController playerMotion
                                         , ICharacterInfo playerInfo
                                         , Animator stateAnimator
                                         , PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(owningAvatar, stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);

        _damagedStateHash = AnimatorState.GetHash(owningAvatar, CurrentState);
        Debug.Assert(StateAnimator.HasState(0, _damagedStateHash), $"Animator of {owningAvatar} does not have {CurrentState} lower state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_damagedStateHash);

        _sternedTime = 0f;

        KnockBack();
    }

    public override void OnUpdate()
    {
        _sternedTime += Time.deltaTime;

        if (_sternedTime < _SternTime) 
            return;

        if (null == PlayerInfo.StandingGround)
            return;

        StateController.ChangeState(LowerStateType.Idle);
        PlayerMotion.SetVelocity(Vector2.zero);
    }


    /****** Private Members ******/

    private const float _SternTime      = 0.4f;
    private const float _KnockBackSpeed = 6f;

    private int _damagedStateHash;
    private float _sternedTime;

    private void KnockBack()
    {
        Vector3 attackerPos = PlayerInfo.RecentDamagedInfo.Attacker.transform.position;
        int direction       = PlayerInfo.CurrentPosition.x > attackerPos.x ? 1 : -1;

        PlayerMotion.SetVelocity(new Vector2(direction * _KnockBackSpeed, _KnockBackSpeed));
    }
}
