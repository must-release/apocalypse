using UnityEngine;
using CharacterEnums;

public class HeroineAnimator : PlayerAnimatorBase
{
    /****** Public Members ******/

    public override void PlayIdle()
    {
        LowerAnimator.SetBool("Move", false);
        UpperAnimator.SetBool("Move", false);
    }

    public override void PlayRunning()
    {
        LowerAnimator.SetBool("Move", true);
        UpperAnimator.SetBool("Move", true);
    }

    public override void PlayJumping()
    {
        LowerAnimator.SetTrigger("Jump");
        UpperAnimator.SetTrigger("Jump");
    }

    /****** Private Members ******/

}