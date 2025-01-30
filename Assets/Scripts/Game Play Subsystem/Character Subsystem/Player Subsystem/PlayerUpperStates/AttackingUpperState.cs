using UnityEngine;
using CharacterEums;

public class AttackingUpperState : PlayerUpperStateBase
{
    private float attackingTime;

    protected override void StartUpperState()
    {
        playerController.AddUpperState(PLAYER_UPPER_STATE.ATTACKING, this);
    }

    public override PLAYER_UPPER_STATE GetState() { return PLAYER_UPPER_STATE.ATTACKING; }

    public override void OnEnter()
    {
        // Execute attack and get attacking motion time
        attackingTime = playerController.CurrentPlayer.Attack();
    }

    public override void OnUpdate()
    {
        // Wait for attacking animation
        attackingTime -= Time.deltaTime;
        if (attackingTime < 0)
        {
            if ( null == playerController.StandingGround )
                playerController.ChangeUpperState(PLAYER_UPPER_STATE.JUMPING);
            else
                playerController.ChangeUpperState(PLAYER_UPPER_STATE.IDLE);
        }
    }

    public override void OnExit(PLAYER_UPPER_STATE _)
    {
        
    }

    public override void LookUp(bool lookUp) 
    { 
        if ( lookUp ) 
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.LOOKING_UP);
    }
    
    public override void Attack() 
    { 
        playerController.ChangeUpperState(PLAYER_UPPER_STATE.ATTACKING); 
    }


    /***** Inavailable State Change *****/
    public override void Stop() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void Move() { return; }
    public override void OnGround() { return; }
    public override void Enable() {return;}
}
