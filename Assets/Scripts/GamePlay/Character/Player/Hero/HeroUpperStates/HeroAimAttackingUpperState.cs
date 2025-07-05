using NUnit.Framework;
using UnityEngine;

public class HeroAimAttackingUpperState : HeroUpperStateBase
{
    /****** Public Members ******/

    public override HeroUpperStateType StateType => HeroUpperStateType.AimAttacking;

    public override void InitializeState(IStateController<HeroUpperStateType> stateController, IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, objectInteractor, playerMotion, playerInfo, stateAnimator, playerWeapon);
        Assert.IsTrue(StateAnimator.HasState(0, _AimAttackingStateHash), "Hero animator does not have aim attacking upper state.");
    }

    public override void OnEnter()
    {
        StateAnimator.Play(_AimAttackingStateHash);

        postDelay = PlayerWeapon.Attack();
    }

    public override void OnUpdate()
    {
        var stateInfo = StateAnimator.GetCurrentAnimatorStateInfo(0);

        postDelay -= Time.deltaTime;

        if (1.0f <= stateInfo.normalizedTime && postDelay < 0)
        {
            StateController.ChangeState(HeroUpperStateType.Aiming);
        }
    }

    public override void OnExit(HeroUpperStateType nextState)
    {
        if (HeroUpperStateType.Aiming != nextState)
        {
            PlayerWeapon.SetWeaponPivotRotation(0);
            PlayerWeapon.Aim(false);
        }
    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim)
            StateController.ChangeState(HeroUpperStateType.Idle);
    }

    public override void Disable()
    {
        StateController.ChangeState(HeroUpperStateType.Disabled);
    }


    /****** Private Members ******/

    private readonly int _AimAttackingStateHash = AnimatorState.Hero.GetHash(HeroUpperStateType.AimAttacking);

    private float postDelay = 0f;
}
