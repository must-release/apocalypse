using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;


public class SplashScreenScene : MonoBehaviour, IScene
{
    public bool CanMoveToNextScene { get; private set; } = false;

    private IEnumerator Start()
    {
        yield return PlaySplashAnimation();

        CanMoveToNextScene = true;
    }

    private IEnumerator PlaySplashAnimation()
    {
        Debug.Log("Splash animation start");

        yield return new WaitForSeconds(2f);
        
        Debug.Log("Splash animation done");
    }
}