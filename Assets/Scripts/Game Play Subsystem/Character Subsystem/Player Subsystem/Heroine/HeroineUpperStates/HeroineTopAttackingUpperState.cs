using UnityEngine;

public class HeroineTopAttackingUpperState : PlayerUpperStateBase<HeroineUpperState>
{
    /****** Public Members ******/

    public override HeroineUpperState StateType => HeroineUpperState.TopAttacking;

    public override void OnEnter()
    {
        // Rotate upper body by 90 degree
        //OwnerController.CurrentAvatar.RotateUpperBody(90);

        // Execute top attack and get attacking motion time
        //_attackCoolTime = OwnerController.CurrentAvatar.Attack();
    }

    public override void OnUpdate()
    {
        // Wait for attacking animation
        _attackCoolTime -= Time.deltaTime;

        if (0 < _attackCoolTime) return;

        StateController.ChangeState(HeroineUpperState.LookingUp);
    }

    public override void OnExit(HeroineUpperState nextState)
    {
        // Recover upper body rotation when not looking up
        if (HeroineUpperState.LookingUp == nextState)
            return;

        //OwnerController.CurrentAvatar.RotateUpperBody(0);
    }

    public override void Attack() 
    { 
        StateController.ChangeState(HeroineUpperState.TopAttacking); 
    }
    
    public override void LookUp(bool lookUp)
    { 
        if(lookUp) return;

        var nextState = PlayerInfo.StandingGround == null ? HeroineUpperState.Disabled : HeroineUpperState.Idle;
        StateController.ChangeState(nextState);
    }

    public override void Disable()
    {
        StateController.ChangeState(HeroineUpperState.Disabled);
    }

    /****** Protected Members ******/

    protected override string AnimationClipPath => AnimationClipAsset.HeroineUpper.TopAttacking;


    /****** Private Members ******/

    private float _attackCoolTime = 0;
}
