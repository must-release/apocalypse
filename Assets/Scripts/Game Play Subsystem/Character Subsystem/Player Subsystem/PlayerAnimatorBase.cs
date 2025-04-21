using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerLowerAnimatorBase : MonoBehaviour
{
    /****** Public Members ******/


    public abstract void PlayIdle();
    public abstract void PlayRunning();


    /****** Protected Members ******/

    protected Animator LowerAnimator => _lowerAnimator;


    /****** Private Members ******/

    private Animator _lowerAnimator;

    private void Awake()
    {
        _lowerAnimator = GetComponent<Animator>();
    }
}

public abstract class PlayerUpperAnimatorBase : MonoBehaviour
{
    /****** Public Members ******/

    public abstract void PlayIdle();
    public abstract void PlayRunning();


    /****** Protected Members ******/

    protected Animator UpperAnimator => _upperAnimator;


    /****** Private Members ******/

    private Animator _upperAnimator;

    private void Awake()
    {
        _upperAnimator = GetComponent<Animator>();
    }
}
