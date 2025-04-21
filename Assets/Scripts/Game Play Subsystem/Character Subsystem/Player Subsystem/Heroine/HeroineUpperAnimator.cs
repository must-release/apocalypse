using UnityEngine;
using CharacterEums;

public class HeroineUpperAnimator : PlayerUpperAnimatorBase
{
    /****** Public Members ******/

    public override void PlayIdle()
    {
        UpperAnimator.SetBool("Move", false);
    }

    public override void PlayRunning()
    {
        UpperAnimator.SetBool("Move", true);
    }

    /****** Private Members ******/

}
