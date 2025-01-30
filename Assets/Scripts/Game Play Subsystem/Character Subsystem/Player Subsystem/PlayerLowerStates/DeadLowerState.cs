using CharacterEums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadLowerState : PlayerLowerStateBase
{
    private const float ANIMATION_PLAYTIME = 0.5f;
    private float animationTime;

    protected override void StartLowerState()
    {
        playerController.AddLowerState(PLAYER_LOWER_STATE.DEAD, this);

        animationTime = 0;
    }

    public override PLAYER_LOWER_STATE GetState() { return PLAYER_LOWER_STATE.DEAD; }

    public override bool DisableUpperBody() { return true; }

    public override void OnEnter()
    {
        animationTime = 0;
    }

    public override void OnUpdate()
    {
        animationTime += Time.deltaTime;
        if( animationTime < ANIMATION_PLAYTIME )
            return;
    }

    public override void OnExit()
    {

    }

    public override void Jump() { return; }
    public override void OnAir() { }
    public override void OnGround() { } 
    public override void Aim(bool isAiming) { return; }
    public override void Move(int move) { return; }
    public override void Damaged() { return; }
    public override void Tag() { return; }
    public override void Climb(bool climb) { return; }
    public override void Stop() { return; }
    public override void Push(bool push) {return;}
    public override void UpDown(int upDown) {return;}
}
