using UnityEngine;
using CharacterEums;

public class AttackingUpperState : PlayerUpperStateBase
{
    private float attackingTime;

    protected override void StartUpperState()
    {
        playerController.RegisterUpperState(PlayerUpperState.ATTACKING, this);
    }

    public override PlayerUpperState GetStateType() { return PlayerUpperState.ATTACKING; }

    public override void OnEnter()
    {
        // Execute attack and get attacking motion time
        attackingTime = playerController.CurrentAvatar.Attack();
    }

    public override void OnUpdate()
    {
        // Wait for attacking animation
        attackingTime -= Time.deltaTime;
        if (attackingTime < 0)
        {
            if ( null == playerController.StandingGround )
                playerController.ChangeUpperState(PlayerUpperState.Jumping);
            else
                playerController.ChangeUpperState(PlayerUpperState.Idle);
        }
    }

    public override void OnExit(PlayerUpperState _)
    {
        
    }

    public override void LookUp(bool lookUp) 
    { 
        if ( lookUp ) 
            playerController.ChangeUpperState(PlayerUpperState.LookingUp);
    }
    
    public override void Attack() 
    { 
        playerController.ChangeUpperState(PlayerUpperState.ATTACKING); 
    }


    /***** Inavailable State Change *****/
    public override void Stop() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void Move() { return; }
    public override void OnGround() { return; }
    public override void Enable() {return;}
}
