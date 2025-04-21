using UnityEngine;
using CharacterEums;

public class TopAttackingUpperState : PlayerUpperStateBase
{
    private float attackCoolTime;

    protected override void StartUpperState()
    {
        playerController.RegisterUpperState(PlayerUpperState.TopAttacking, this);

        attackCoolTime = 0;
    }

    public override PlayerUpperState GetStateType() { return PlayerUpperState.TopAttacking; }

    public override void OnEnter()
    {
        // Rotate upper body by 90 degree
        playerController.CurrentAvatar.RotateUpperBody(90);

        // Execute top attack and get attacking motion time
        attackCoolTime = playerController.CurrentAvatar.Attack();
    }

    public override void OnUpdate()
    {
        // Wait for attacking animation
        attackCoolTime -= Time.deltaTime;
        if ( attackCoolTime < 0 )
            playerController.ChangeUpperState(PlayerUpperState.LookingUp);
    }

    public override void OnExit(PlayerUpperState nextState)
    {
        // Recover upper body rotation when not looking up
        if (PlayerUpperState.LookingUp == nextState)
            return;

        playerController.CurrentAvatar.RotateUpperBody(0);
    }

    public override void Attack() 
    { 
        playerController.ChangeUpperState(PlayerUpperState.TopAttacking); 
    }
    
    public override void LookUp(bool lookUp)
    { 
        if( lookUp ) return;

        if (playerController.StandingGround )
            playerController.ChangeUpperState(PlayerUpperState.Idle);
        else
            playerController.ChangeUpperState(PlayerUpperState.Jumping);
    }


    /***** Inavailable State Change *****/
    public override void Stop() { return; }
    public override void Jump() { return; }
    public override void OnAir() { return; }
    public override void Move() { return; }
    public override void OnGround() { return; }
    public override void Enable() {return;}
}
