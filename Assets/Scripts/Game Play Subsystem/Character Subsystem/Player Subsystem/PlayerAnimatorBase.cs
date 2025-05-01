using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class PlayerAnimatorBase : MonoBehaviour
{
    /****** Public Members ******/

    public abstract void PlayIdle();
    public abstract void PlayRunning();
    public abstract void PlayJumping();


    /****** Protected Members ******/

    protected Animator LowerAnimator => _lowerAnimator;
    protected Animator UpperAnimator => _upperAnimator;


    /****** Private Members ******/

    [SerializeField] private Animator _lowerAnimator = null;
    [SerializeField] private Animator _upperAnimator = null;

    private void Awake()
    {
        Assert.IsTrue(null != _lowerAnimator, "Lower Animator is not assigned.");
        Assert.IsTrue(null != _upperAnimator, "Upper Animator is not assigned.");

    }
}
