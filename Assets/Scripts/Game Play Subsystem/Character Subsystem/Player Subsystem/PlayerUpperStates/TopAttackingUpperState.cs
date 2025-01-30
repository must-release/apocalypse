using UnityEngine;
using CharacterEums;

public class TopAttackingUpperState : PlayerUpperStateBase
{
    private float attackCoolTime;

    protected override void StartUpperState()
    {
        playerController.AddUpperState(PLAYER_UPPER_STATE.TOP_ATTACKING, this);

        attackCoolTime = 0;
    }

    public override PLAYER_UPPER_STATE GetState() { return PLAYER_UPPER_STATE.TOP_ATTACKING; }

    public override void OnEnter()
    {
        // Rotate upper body by 90 degree
        playerController.CurrentPlayer.RotateUpperBody(90);

        // Execute top attack and get attacking motion time
        attackCoolTime = playerController.CurrentPlayer.Attack();
    }

    public override void OnUpdate()
    {
        // Wait for attacking animation
        attackCoolTime -= Time.deltaTime;
        if ( attackCoolTime < 0 )
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.LOOKING_UP);
    }

    public override void OnExit(PLAYER_UPPER_STATE nextState)
    {
        // Recover upper body rotation when not looking up
        if (PLAYER_UPPER_STATE.LOOKING_UP == nextState)
            return;

        playerController.CurrentPlayer.RotateUpperBody(0);
    }

    public override void Attack() 
    { 
        playerController.ChangeUpperState(PLAYER_UPPER_STATE.TOP_ATTACKING); 
    }
    
    public override void LookUp(bool lookUp)
    { 
        if( lookUp ) return;

        if (playerController.StandingGround )
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.IDLE);
        else
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.JUMPING);
    }


    /***** Inavailable State Change *****/
    public override void Stop() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void Move() { return; }
    public override void OnGround() { return; }
    public override void Enable() {return;}
}
