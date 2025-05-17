using UnityEngine;

public class HeroineIdleUpperState : HeroineUpperStateBase
{
    public override HeroineUpperState StateType => HeroineUpperState.Idle;

    public override void OnEnter()
    {
        StateAnimator.Play(AnimatorState.HeroineUpper.Idle);
    }
    public override void OnUpdate()
    {

    }
    public override void OnExit(HeroineUpperState _)
    {

    }

    public override void Move()
    {
        StateController.ChangeState(HeroineUpperState.Running); 
    }

    public override void LookUp(bool lookUp) 
    { 
        if (false == lookUp) return;

        StateController.ChangeState(HeroineUpperState.LookingUp);
    }

    public override void Disable()
    {
        StateController.ChangeState(HeroineUpperState.Disabled);
    }



    /****** Protected Members ******/

    protected override string AnimationClipPath => AnimationClipAsset.HeroineUpper.Idle;
}
