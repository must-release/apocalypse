using UnityEngine;
using CharacterEums;

public class AttackingUpperState : PlayerUpperStateBase
{
    private float attackingTime;

    public override CommonPlayerUpperState GetStateType() { return CommonPlayerUpperState.Attacking; }

    public override void OnEnter()
    {
        // Execute attack and get attacking motion time
        attackingTime = OwnerController.CurrentAvatar.Attack();
    }

    public override void OnUpdate()
    {
        // Wait for attacking animation
        attackingTime -= Time.deltaTime;
        if (attackingTime < 0)
        {
            if ( null == OwnerController.StandingGround )
                OwnerController.ChangeUpperState(CommonPlayerUpperState.Jumping);
            else
                OwnerController.ChangeUpperState(CommonPlayerUpperState.Idle);
        }
    }

    public override void OnExit(CommonPlayerUpperState _)
    {
        
    }

    public override void LookUp(bool lookUp) 
    { 
        if ( lookUp ) 
            OwnerController.ChangeUpperState(CommonPlayerUpperState.LookingUp);
    }
    
    public override void Attack() 
    { 
        OwnerController.ChangeUpperState(CommonPlayerUpperState.Attacking); 
    }


    /***** Inavailable State Change *****/
    public override void Stop() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void Move() { return; }
    public override void OnGround() { return; }
    public override void Enable() {return;}
}
