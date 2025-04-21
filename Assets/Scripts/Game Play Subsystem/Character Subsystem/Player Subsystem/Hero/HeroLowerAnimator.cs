using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLowerAnimator : PlayerLowerAnimatorBase
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

    /****** Private Members ******/
}
