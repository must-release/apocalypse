using UnityEngine;

public class HeroineLookingUpUpperState : PlayerUpperStateBase<HeroineUpperState>
{

    public override HeroineUpperState StateType => HeroineUpperState.LookingUp;

    public override void OnEnter()
    {
        // Rotate upper body by 90 degree
        //OwnerController.CurrentAvatar.RotateUpperBody(90);
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit(HeroineUpperState nextState)
    {
        // Recover upper body rotation when not attacking
        if (HeroineUpperState.TopAttacking == nextState)
            return;
        
        //OwnerController.CurrentAvatar.RotateUpperBody(0);
    }

    public override void LookUp(bool lookUp) 
    { 
        if(lookUp) return;

        var nextState = PlayerInfo.StandingGround == null ? HeroineUpperState.Disabled : HeroineUpperState.Idle;
        StateController.ChangeState(nextState);
    }

    public override void Attack() 
    { 
        StateController.ChangeState(HeroineUpperState.TopAttacking);
    }
    public override void Disable()
    {
        StateController.ChangeState(HeroineUpperState.Disabled);
    }

    /****** Protected Members ******/

    protected override string AnimationClipPath => AnimationClipAsset.HeroineUpper.LookingUp;
}
