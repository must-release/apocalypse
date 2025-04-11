using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;


public class SplashScreen : MonoBehaviour
{
    private bool _frameworkInitialized = false;

    private IEnumerator Start()
    {
        yield return PlaySplashAnimation();

        yield return new WaitUntil(() => _frameworkInitialized);

        SubmitEvent();
    }

    private IEnumerator PlaySplashAnimation()
    {
        Debug.Log("Splash animation start");

        yield return new WaitForSeconds(2f);
        
        Debug.Log("Splash animation done");
    }

    public void OnFrameworkInitialized()
    {
        _frameworkInitialized = true;
    }

    private void SubmitEvent()
    {
        Assert.IsTrue(_frameworkInitialized, "Framework not initialized");

        GameEventManager.Instance.Submit(GameEventFactory.CreateSequentialEvent( new List<GameEvent> {
            GameEventFactory.CreateSceneLoadEvent(SceneEnums.SceneName.TitleScene),
            GameEventFactory.CreateUIChangeEvent(UIEnums.BaseUI.Title),
            GameEventFactory.CreateSceneActivateEvent(false)
        }));
    }
}