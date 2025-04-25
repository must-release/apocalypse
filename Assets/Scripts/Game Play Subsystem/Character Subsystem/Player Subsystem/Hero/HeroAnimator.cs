using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnimator : PlayerAnimatorBase
{
    /****** Public Members ******/

    public override void PlayIdle()
    {
        LowerAnimator.SetBool("Move", false);
    }

    public override void PlayRunning()
    {
        LowerAnimator.SetBool("Move", true);
    }

    public override void PlayJumping()
    {
        LowerAnimator.SetTrigger("Jump");

    }

    /****** Private Members ******/
}
