using NUnit.Framework;
using UnityEngine;

public class HeroAimAttackingUpperState : HeroUpperStateBase
{
    /****** Public Members ******/

    public override HeroUpperState StateType => HeroUpperState.AimAttacking;

    public override void InitializeState(IStateController<HeroUpperState> stateController, IMotionController playerMotion, ICharacterInfo playerInfo, Animator stateAnimator, PlayerWeaponBase playerWeapon)
    {
        base.InitializeState(stateController, playerMotion, playerInfo, stateAnimator, playerWeapon);
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
            StateController.ChangeState(HeroUpperState.Aiming);
        }
    }

    public override void OnExit(HeroUpperState nextState)
    {
        if (HeroUpperState.Aiming != nextState)
        {
            PlayerWeapon.SetWeaponPivotRotation(0);
            PlayerWeapon.Aim(false);
        }
    }

    public override void Aim(Vector3 aim)
    {
        if (Vector3.zero == aim)
            StateController.ChangeState(HeroUpperState.Idle);
    }

    public override void Disable()
    {
        StateController.ChangeState(HeroUpperState.Disabled);
    }


    /****** Private Members ******/

    private readonly int _AimAttackingStateHash = AnimatorState.Hero.GetHash(HeroUpperState.AimAttacking);

    private float postDelay = 0f;
}
