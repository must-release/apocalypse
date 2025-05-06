using UnityEngine;

public class HeroTopAttackingUpperState : PlayerUpperStateBase<HeroUpperState>
{
    /****** Public Members ******/

    public override HeroUpperState StateType => HeroUpperState.TopAttacking;

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

        StateController.ChangeState(HeroUpperState.LookingUp);
    }

    public override void OnExit(HeroUpperState nextState)
    {
        // Recover upper body rotation when not looking up
        if (HeroUpperState.LookingUp == nextState)
            return;

        //OwnerController.CurrentAvatar.RotateUpperBody(0);
    }

    public override void Attack() 
    { 
        StateController.ChangeState(HeroUpperState.TopAttacking); 
    }
    
    public override void LookUp(bool lookUp)
    { 
        if(lookUp) return;

        var nextState = PlayerInfo.StandingGround == null ? HeroUpperState.Jumping : HeroUpperState.Idle;
        StateController.ChangeState(nextState);
    }

    /****** Protected Members ******/

    protected override string AnimationClipPath => "";

    /****** Private Members ******/

    private float _attackCoolTime = 0;
}
