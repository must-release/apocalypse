using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingLowerState : PlayerLowerStateBase
{
    protected override void StartLowerState()
    {
        playerController.AddLowerState(PLAYER_LOWER_STATE.AIMING, this);
    }

    public override PLAYER_LOWER_STATE GetState() { return PLAYER_LOWER_STATE.AIMING; }

    public override bool DisableUpperBody() { return false; }

    public override void OnEnter()
    {
        playerRigid.velocity = Vector2.zero;
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {

    }

    public override void Aim(bool isAiming) 
    { 
        if(!isAiming) 
            playerController.ChangeLowerState(PLAYER_LOWER_STATE.IDLE);
    }

    public override void OnAir() 
    {
        playerController.ChangeLowerState(PLAYER_LOWER_STATE.JUMPING); 
    }
    
    public override void Move(int move) { return; }
    public override void Climb(bool climb) { return; }
    public override void Jump() {return;}
    public override void Tag() { return; }
    public override void OnGround() { return; }
    public override void Stop() { return; }
    public override void Push(bool push) { return; }
    public override void UpDown(int upDown) { return; }
}
